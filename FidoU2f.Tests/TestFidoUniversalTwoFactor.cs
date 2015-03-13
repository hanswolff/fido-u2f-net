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
using System.Text;
using FidoU2f.Models;
using Moq;
using NUnit.Framework;

namespace FidoU2f.Tests
{
	[TestFixture]
	public class TestFidoUniversalTwoFactor
	{
		[Test]
		public void StartRegistration()
		{
			var randomChallenge = Encoding.Default.GetBytes("random challenge");

			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(randomChallenge);

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);

			mockGenerateChallenge.Verify(x => x.GenerateChallenge(), Times.Once);

			Assert.AreEqual(TestVectors.AppIdEnroll, startedRegistration.AppId.ToString());
			Assert.AreEqual(randomChallenge, WebSafeBase64Converter.FromBase64String(startedRegistration.Challenge));
		}

		[Test]
		public void FinishRegistration_JsonRegisterResponse_Works()
		{
            var challenge = WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeRegisterBase64);

            var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
            mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(challenge);

            var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);
			startedRegistration.Challenge = TestVectors.ServerChallengeRegisterBase64;

			var registerResponse = GetValidRegisterResponse();
			var registrationData = registerResponse.RegistrationData;

		    var jsonValue = registerResponse.ToJson();
		    var deviceRegistration = fido.FinishRegistration(startedRegistration, jsonValue, TestVectors.TrustedDomains);
			Assert.IsNotNull(deviceRegistration);
			Assert.AreEqual(deviceRegistration.Certificate.RawData, registrationData.AttestationCertificate.RawData);
			Assert.AreEqual(deviceRegistration.KeyHandle, registrationData.KeyHandle);
		}

		[Test]
		public void FinishRegistration_RegisterResponse_Works()
		{
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);
			startedRegistration.Challenge = TestVectors.ServerChallengeRegisterBase64;

			var registerResponse = GetValidRegisterResponse();
			var registrationData = registerResponse.RegistrationData;

			var deviceRegistration = fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains);
			Assert.IsNotNull(deviceRegistration);
			Assert.AreEqual(deviceRegistration.Certificate.RawData, registrationData.AttestationCertificate.RawData);
			Assert.AreEqual(deviceRegistration.KeyHandle, registrationData.KeyHandle);
		}

		[Test]
		public void FinishRegistration_IncorrectType_Throws()
		{
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);

			var registerResponse = GetValidRegisterResponse();
			registerResponse.ClientData.Type = "incorrect type";

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains));
		}

		[Test]
		public void FinishRegistration_IncorrectChallenge_Throws()
		{
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);

			var registerResponse = GetValidRegisterResponse();
			registerResponse.ClientData.Challenge =
				WebSafeBase64Converter.ToBase64String(Encoding.Default.GetBytes("incorrect challenge"));

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains));
		}

		[TestCase("http://not.trusted")]
		[TestCase("http://example.com:81")]
		[TestCase("http://www.example.com")]
		[TestCase("https://example.com")]
		public void FinishRegistration_UntrustedOrigin_Throws(string origin)
		{
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);

			var registerResponse = GetValidRegisterResponse();
			registerResponse.ClientData.Origin = origin;

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains));
		}

		[Test]
		public void StartAuthentication()
		{
			var randomChallenge = Encoding.Default.GetBytes("random challenge");

			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(randomChallenge);

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			mockGenerateChallenge.Verify(x => x.GenerateChallenge(), Times.Once);

			Assert.AreEqual(TestVectors.AppIdEnroll, startedAuthentication.AppId.ToString());
			Assert.AreEqual(randomChallenge, WebSafeBase64Converter.FromBase64String(startedAuthentication.Challenge));
			Assert.AreEqual(deviceRegistration.KeyHandle, startedAuthentication.KeyHandle);
		}

		[Test]
		public void FinishAuthentication_Works()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(TestVectors.ClientDataAuth),
				FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64),
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains);
		}

		[Test]
		public void FinishAuthentication_DifferentChallenge()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var clientDataAuth = TestVectors.ClientDataAuth.Replace("challenge\":\"opsXqUifDriAAmWclinfbS0e-USY0CgyJHe_Otd7z8o", "challenge\":\"different");

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(clientDataAuth),
				FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64),
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		[Test]
		public void FinishAuthentication_DifferentType()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var clientDataAuth = TestVectors.ClientDataAuth.Replace("typ\":\"navigator.id.getAssertion", "typ\":\"different");

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(clientDataAuth),
				FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64),
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		[TestCase("http://example.com/subpath")]
		[TestCase("http://example.com:80")]
		[TestCase("http://example.com:80/subpath")]
		public void FinishAuthentication_TrustedOrigin(string origin)
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var clientDataAuth = TestVectors.ClientDataAuth.Replace("origin\":\"http://example.com", "origin\":\"" + origin);

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(clientDataAuth),
				FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64),
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains);
		}

		[TestCase("http://not.trusted")]
		[TestCase("http://example.com:81")]
		[TestCase("http://www.example.com")]
		[TestCase("https://example.com")]
		public void FinishAuthentication_UntrustedOrigin(string origin)
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var clientDataAuth = TestVectors.ClientDataAuth.Replace("origin\":\"http://example.com", "origin\":\"" + origin);

            var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(clientDataAuth),
				FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64),
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		[Test]
		public void FinishAuthentication_CounterTooSmall()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var signatureData = FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64);
			signatureData = new FidoSignatureData(
				signatureData.UserPresence,
				0,
				signatureData.Signature);

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(TestVectors.ClientDataAuth),
				signatureData,
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		[Test]
		public void FinishAuthentication_UserPresenceNotSet()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var signatureData = FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64);
			signatureData = new FidoSignatureData(
				0,
				signatureData.Counter,
				signatureData.Signature);

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(TestVectors.ClientDataAuth),
				signatureData,
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		[Test]
		public void FinishAuthentication_InvalidSignatureData()
		{
			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(WebSafeBase64Converter.FromBase64String(TestVectors.ServerChallengeAuthBase64));

			var fido = new FidoUniversalTwoFactor(mockGenerateChallenge.Object);

			var deviceRegistration = CreateTestDeviceRegistration();
			var startedAuthentication = fido.StartAuthentication(new FidoAppId(TestVectors.AppIdEnroll), deviceRegistration);

			var signatureData = FidoSignatureData.FromWebSafeBase64(TestVectors.SignResponseDataBase64);
			var signatureBytes = signatureData.Signature.ToByteArray();
			signatureBytes[0] ^= 0xFF;

			signatureData = new FidoSignatureData(
				signatureData.UserPresence,
				signatureData.Counter,
				new FidoSignature(signatureBytes));

			var authenticateResponse = new FidoAuthenticateResponse(
				FidoClientData.FromJson(TestVectors.ClientDataAuth),
				signatureData,
				FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle));

			Assert.Throws<InvalidOperationException>(() => fido.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration, TestVectors.TrustedDomains));
		}

		private static FidoRegisterResponse GetValidRegisterResponse()
		{
			var registerResponse = new FidoRegisterResponse
			{
				RegistrationData = FidoRegistrationData.FromWebSafeBase64(TestVectors.RegistrationResponseDataBase64),
				ClientData = FidoClientData.FromJson(TestVectors.ClientDataRegister)
			};
			return registerResponse;
		}

		private static FidoDeviceRegistration CreateTestDeviceRegistration()
		{
			var cert = FidoAttestationCertificate.FromWebSafeBase64(TestVectors.AttestationCertificate);
			var keyHandle = FidoKeyHandle.FromWebSafeBase64(TestVectors.KeyHandle);
			var publicKey = FidoPublicKey.FromWebSafeBase64(TestVectors.PublicKey);
			return new FidoDeviceRegistration(keyHandle, publicKey, cert, 0);
		}
	}
}
