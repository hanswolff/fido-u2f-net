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
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);
			startedRegistration.Challenge = TestVectors.ServerChallengeRegisterBase64;

			var registerResponse = GetValidRegisterResponse();
			var registrationData = registerResponse.GetParsedRegistrationData();

			var deviceRegistration = fido.FinishRegistration(startedRegistration, registerResponse.ToJson(), TestVectors.TrustedDomains);
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
			var registrationData = registerResponse.GetParsedRegistrationData();

			var deviceRegistration = fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains);
			Assert.IsNotNull(deviceRegistration);
			Assert.AreEqual(deviceRegistration.Certificate.RawData, registrationData.AttestationCertificate.RawData);
			Assert.AreEqual(deviceRegistration.KeyHandle, registrationData.KeyHandle);
		}

		private static FidoRegisterResponse GetValidRegisterResponse()
		{
			var registerResponse = new FidoRegisterResponse
			{
				RegistrationDataAsBase64 = TestVectors.RegistrationResponseDataBase64,
				ClientData = FidoClientData.FromJson(TestVectors.ClientDataRegister)
			};
			return registerResponse;
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

		[Test]
		public void FinishRegistration_UntrustedOrigin_Throws()
		{
			var fido = new FidoUniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestVectors.AppIdEnroll);

			var registerResponse = GetValidRegisterResponse();
			registerResponse.ClientData.Origin = "http://not.trusted";

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, TestVectors.TrustedDomains));
		}
    }
}
