//
// The MIT License(MIT)
//
// Copyright(c) 2015 Hans Wolff
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using FidoU2f.Models;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;

namespace FidoU2f
{
	public class FidoUniversalTwoFactor : IFidoUniversalTwoFactor
	{
		public static readonly string Version = "U2F_V2";

		public const string AuthenticateType = "navigator.id.getAssertion";
		public const string RegisterType = "navigator.id.finishEnrollment";

		public static byte UserPresentFlag = 0x01;

		private readonly IGenerateFidoChallenge _generateFidoChallenge;

		public FidoUniversalTwoFactor()
			: this(null)
		{
		}

		public FidoUniversalTwoFactor(IGenerateFidoChallenge generateFidoChallenge)
		{
			_generateFidoChallenge = generateFidoChallenge ?? new GenerateRandomFidoChallenge();
		}

		public FidoStartedRegistration StartRegistration(string appId)
		{
			return StartRegistration(new FidoAppId(appId));
		}

		public FidoStartedRegistration StartRegistration(FidoAppId appId)
		{
			var challengeBytes = _generateFidoChallenge.GenerateChallenge();
			var challenge = WebSafeBase64Converter.ToBase64String(challengeBytes);

			return new FidoStartedRegistration(appId, challenge);
		}

		public FidoDeviceRegistration FinishRegistration(FidoStartedRegistration startedRegistration, 
			string jsonDeviceResponse, IEnumerable<FidoFacetId> trustedFacetIds)
		{
			if (jsonDeviceResponse == null) throw new ArgumentNullException("jsonDeviceResponse");

			var registerResponse = FidoRegisterResponse.FromJson(jsonDeviceResponse);
			return FinishRegistration(startedRegistration, registerResponse, trustedFacetIds);
		}

		public FidoDeviceRegistration FinishRegistration(FidoStartedRegistration startedRegistration, 
			FidoRegisterResponse registerResponse, IEnumerable<FidoFacetId> trustedFacetIds)
		{
			if (startedRegistration == null) throw new ArgumentNullException("startedRegistration");
			if (registerResponse == null) throw new ArgumentNullException("registerResponse");
			if (trustedFacetIds == null) throw new ArgumentNullException("trustedFacetIds");

			registerResponse.Validate();

			var clientData = registerResponse.ClientData;

			ExpectClientDataType(clientData, RegisterType);

			if (clientData.Challenge != startedRegistration.Challenge)
				throw new InvalidOperationException("Incorrect challenge signed in client data");

            ValidateOrigin(trustedFacetIds, new FidoFacetId(clientData.Origin));

			var registrationData = registerResponse.RegistrationData;
			VerifyResponseSignature(startedRegistration.AppId, registrationData, clientData);

			return new FidoDeviceRegistration(registrationData.KeyHandle, registrationData.UserPublicKey,
				registrationData.AttestationCertificate, 0);
		}

		private static void ValidateOrigin(IEnumerable<FidoFacetId> trustedFacetIds, FidoFacetId origin)
		{
			if (!trustedFacetIds.Any(x => x.Equals(origin)))
				throw new InvalidOperationException(String.Format("{0} is not a recognized trusted origin for this backend", origin));
		}

		private static void ExpectClientDataType(FidoClientData clientData, string expectedType)
		{
			if (clientData.Type == expectedType) return;

			var message = String.Format("Unexpected type in client data (expected '{0}' but was '{1}')",
				expectedType, clientData.Type);
			throw new InvalidOperationException(message);
		}

		private void VerifyResponseSignature(FidoAppId appId, FidoRegistrationData registrationData, FidoClientData clientData)
		{
			if (appId == null) throw new ArgumentNullException("appId");
			if (registrationData == null) throw new ArgumentNullException("registrationData");
			if (clientData == null) throw new ArgumentNullException("clientData");

			if (String.IsNullOrEmpty(clientData.RawJsonValue))
				throw new InvalidOperationException("Client data has no JSON representation");

			var signedBytes = PackBytes(
				new byte[] { 0 },
				Sha256(appId.ToString()),
				Sha256(clientData.RawJsonValue),
				registrationData.KeyHandle.ToByteArray(),
				registrationData.UserPublicKey.ToByteArray());

			VerifySignature(registrationData.AttestationCertificate, registrationData.Signature, signedBytes);
		}

		private void VerifySignature(FidoAttestationCertificate certificate, FidoSignature signature, 
			byte[] signedBytes)
		{
			try
			{
				var certPublicKey = certificate.Certificate.GetPublicKey();
				var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(false, certPublicKey);
				signer.BlockUpdate(signedBytes, 0, signedBytes.Length);

				if (signer.VerifySignature(signature.ToByteArray()))
					throw new InvalidOperationException("Invalid signature");
			}
			catch (Exception)
			{
				throw new InvalidOperationException("Invalid signature");
			}
		}

		private byte[] Sha256(string text)
		{
			var bytes = new byte[text.Length * sizeof(char)];
			Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);

			var sha256 = new Sha256Digest();
			var hash = new byte[sha256.GetDigestSize()];
			sha256.BlockUpdate(bytes, 0, bytes.Length);
			sha256.DoFinal(hash, 0);
			return hash;
		}

		private static byte[] PackBytes(params byte[][] bytes)
		{
			var result = new List<byte>();

			foreach (var chunk in bytes)
				result.AddRange(chunk);

			return result.ToArray();
		}

		public FidoStartedAuthentication StartAuthentication(FidoAppId appId, FidoDeviceRegistration deviceRegistration)
		{
			if (appId == null) throw new ArgumentNullException("appId");
			if (deviceRegistration == null) throw new ArgumentNullException("deviceRegistration");

			var challenge = _generateFidoChallenge.GenerateChallenge();

			return new FidoStartedAuthentication(appId, 
				WebSafeBase64Converter.ToBase64String(challenge),
				deviceRegistration.KeyHandle);
		}

	    public uint FinishAuthentication(FidoStartedAuthentication startedAuthentication,
	        string rawAuthResponse,
	        FidoDeviceRegistration deviceRegistration,
	        IEnumerable<FidoFacetId> trustedFacetIds)
	    {
	        var authResponse = FidoAuthenticateResponse.FromJson(rawAuthResponse);
	        return FinishAuthentication(startedAuthentication, authResponse, deviceRegistration, trustedFacetIds);
	    }

	    public uint FinishAuthentication(FidoStartedAuthentication startedAuthentication,
			FidoAuthenticateResponse authResponse,
			FidoDeviceRegistration deviceRegistration,
			IEnumerable<FidoFacetId> trustedFacetIds)
		{
			authResponse.Validate();

			var clientData = authResponse.ClientData;

			ExpectClientDataType(clientData, AuthenticateType);

			if (clientData.Challenge != startedAuthentication.Challenge)
				throw new InvalidOperationException("Incorrect challenge signed in client data");

			ValidateOrigin(trustedFacetIds, new FidoFacetId(clientData.Origin));

			var signatureData = authResponse.SignatureData;

			VerifyAuthSignature(startedAuthentication.AppId, signatureData, clientData, deviceRegistration);

			deviceRegistration.UpdateCounter(signatureData.Counter);
			return signatureData.Counter;
		}

		private void VerifyAuthSignature(FidoAppId appId, FidoSignatureData signatureData, FidoClientData clientData, 
			FidoDeviceRegistration deviceRegistration)
		{
			if (appId == null) throw new ArgumentNullException("appId");
			if (signatureData == null) throw new ArgumentNullException("signatureData");
			if (clientData == null) throw new ArgumentNullException("clientData");
			if (deviceRegistration == null) throw new ArgumentNullException("deviceRegistration");

			if (String.IsNullOrEmpty(clientData.RawJsonValue))
				throw new InvalidOperationException("Client data has no JSON representation");

			var counterBytes = BitConverter.GetBytes(signatureData.Counter);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(counterBytes);

			var signedBytes = PackBytes(
				Sha256(appId.ToString()),
				new [] { signatureData.UserPresence },
				counterBytes,
				Sha256(clientData.RawJsonValue));

			VerifySignature(deviceRegistration, signatureData.Signature, signedBytes);

			if (signatureData.UserPresence != UserPresentFlag)
				throw new InvalidOperationException("User presence invalid during authentication");
		}

		private void VerifySignature(FidoDeviceRegistration deviceRegistration, FidoSignature signature, 
			byte[] signedBytes)
		{
			try
			{
				var certPublicKey = deviceRegistration.PublicKey.PublicKey;
				var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(false, certPublicKey);
				signer.BlockUpdate(signedBytes, 0, signedBytes.Length);

				if (signer.VerifySignature(signature.ToByteArray()))
					throw new InvalidOperationException("Invalid signature");
			}
			catch
			{
				throw new InvalidOperationException("Invalid signature");
			}
		}
	}
}
