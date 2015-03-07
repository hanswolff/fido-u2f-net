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
using System.IO;

namespace FidoU2f.Models
{
	public class FidoRegistrationData
	{
		private const byte RegistrationReservedByte = 0x05;

		/// <summary>
		/// The (uncompressed) x,y-representation of a curve point on the P-256 NIST elliptic curve.
		/// </summary>
		public FidoPublicKey UserPublicKey { get; private set; }

		/// <summary>
		/// A handle that allows the U2F token to identify the generated key pair.
		/// </summary>
		public FidoKeyHandle KeyHandle { get; private set; }

		public FidoAttestationCertificate AttestationCertificate { get; private set; }

		/// <summary>
		/// A ECDSA signature (on P-256)
		/// </summary>
		public FidoSignature Signature { get; private set; }

		private FidoRegistrationData(FidoPublicKey userPublicKey, FidoKeyHandle keyHandle,
						   FidoAttestationCertificate attestationCertificate,
						   FidoSignature signature)
		{
			UserPublicKey = userPublicKey;
			KeyHandle = keyHandle;
			AttestationCertificate = attestationCertificate;
			Signature = signature;
		}

		public static FidoRegistrationData FromString(string webSafeBase64)
		{
			return FromBytes(WebSafeBase64Converter.FromBase64String(webSafeBase64));
		}

		public static FidoRegistrationData FromBytes(byte[] rawRegistrationData)
		{
			using (var mem = new MemoryStream(rawRegistrationData))
			{
				return FromStream(mem);
			}
		}

		private static FidoRegistrationData FromStream(Stream stream)
		{
			using (var binaryReader = new BinaryReader(stream))
			{
				var reservedByte = binaryReader.ReadByte();

				if (reservedByte != RegistrationReservedByte)
				{
					throw new InvalidOperationException(String.Format(
						"Incorrect value of reserved byte (expected: 0x{0:X2} but was: 0x{1:X1})",
						RegistrationReservedByte, reservedByte));
				}

				try
				{
					var publicKeyBytes = binaryReader.ReadBytes(65);
					var keyHandleBytes = binaryReader.ReadBytes(binaryReader.ReadByte());

					var nextChunkSize = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
					var certificatePosition = binaryReader.BaseStream.Position;
					var certBytes = binaryReader.ReadBytes(nextChunkSize);
					var certificate = new FidoAttestationCertificate(certBytes);
					var certSize = certificate.Certificate.GetEncoded().Length;

					binaryReader.BaseStream.Position = certificatePosition + certSize;
					nextChunkSize = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);

					var signatureBytes = binaryReader.ReadBytes(nextChunkSize);

					var registerResponse = new FidoRegistrationData(
						new FidoPublicKey(publicKeyBytes),
						new FidoKeyHandle(keyHandleBytes),
						certificate,
						new FidoSignature(signatureBytes));

					return registerResponse;
				}
				catch (Exception ex)
				{
					var message = String.Format("Error parsing registration data ({0})", ex.Message);
					throw new InvalidOperationException(message, ex);
				}
			}
		}
	}
}
