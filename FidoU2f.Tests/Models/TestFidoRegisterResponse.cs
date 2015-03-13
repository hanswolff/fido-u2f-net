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
using FidoU2f.Models;
using NUnit.Framework;

namespace FidoU2f.Tests.Models
{
	[TestFixture]
	public class TestFidoRegisterResponse
	{
	    [Test]
	    public void FromJson()
	    {
	        var json =
	            "{ \"registrationData\":\"BQQcH08Pj414DUayxwcgVU4Z5NXfkFeWz51FT-g5gW7GjPWprecrSECJBj1oNsEjkJynnFkIwZLL4NC6M46GnUopQGJUaobtq9pUKffgVYPJDDLHuAQZLoDz390Z5uwbdFkzYItk3L270bBIM-k0VO1DKF25SGP2XK9NZv-qxWQrV_gwggIcMIIBBqADAgECAgQk26tAMAsGCSqGSIb3DQEBCzAuMSwwKgYDVQQDEyNZdWJpY28gVTJGIFJvb3QgQ0EgU2VyaWFsIDQ1NzIwMDYzMTAgFw0xNDA4MDEwMDAwMDBaGA8yMDUwMDkwNDAwMDAwMFowKzEpMCcGA1UEAwwgWXViaWNvIFUyRiBFRSBTZXJpYWwgMTM1MDMyNzc4ODgwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAAQCsJS-NH1HeUHEd46-xcpN7SpHn6oeb-w5r-veDCBwy1vUvWnJanjjv4dR_rV5G436ysKUAXUcsVe5fAnkORo2oxIwEDAOBgorBgEEAYLECgEBBAAwCwYJKoZIhvcNAQELA4IBAQCjY64OmDrzC7rxLIst81pZvxy7ShsPy2jEhFWEkPaHNFhluNsCacNG5VOITCxWB68OonuQrIzx70MfcqwYnbIcgkkUvxeIpVEaM9B7TI40ZHzp9h4VFqmps26QCkAgYfaapG4SxTK5k_lCPvqqTPmjtlS03d7ykkpUj9WZlVEN1Pf02aTVIZOHPHHJuH6GhT6eLadejwxtKDBTdNTv3V4UlvjDOQYQe9aL1jUNqtLDeBHso8pDvJMLc0CX3vadaI2UVQxM-xip4kuGouXYj0mYmaCbzluBDFNsrzkNyL3elg3zMMrKvAUhoYMjlX_-vKWcqQsgsQ0JtSMcWMJ-umeDMEYCIQCKz0k0wQVooa638uF67HNyqNaa2vL5A-LkCDrLV5v74QIhAPjJ8UrQVCymLS1xoLWTw0CPD5U7DuerMqHgsvEnMR3c\",\"challenge\":\"V8D5zRbaeY3Dl_AR5U4g3eZb_HCpZgd17Jeh1LqacL0\",\"version\":\"U2F_V2\",\"appId\":\"http://localhost:3214\",\"clientData\":\"eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZmluaXNoRW5yb2xsbWVudCIsImNoYWxsZW5nZSI6IlY4RDV6UmJhZVkzRGxfQVI1VTRnM2VaYl9IQ3BaZ2QxN0plaDFMcWFjTDAiLCJvcmlnaW4iOiJodHRwOi8vbG9jYWxob3N0OjMyMTQiLCJjaWRfcHVia2V5IjoiIn0\"}";

	        FidoRegisterResponse.FromJson(json);
	    }

		[Test]
		public void Validate_Wellformed_NoException()
		{
			var registerResponse = CreateGoodRegisterResponse();
			registerResponse.Validate();
		}

		[Test]
		public void Validate_RegistrationDataMissing_Throws()
		{
			var registerResponse = CreateGoodRegisterResponse();
			registerResponse.RegistrationData = null;

			Assert.Throws<InvalidOperationException>(() => registerResponse.Validate());
		}

        [Test]
        public void Validate_ClientDataMissing_Throws()
        {
            var registerResponse = CreateGoodRegisterResponse();
            registerResponse.ClientData = null;

            Assert.Throws<InvalidOperationException>(() => registerResponse.Validate());
        }

        [Test]
		public void Validate_ClientDataChallengeMissing_Throws()
		{
			var registerResponse = CreateGoodRegisterResponse();
			registerResponse.ClientData.Challenge = "";

			Assert.Throws<InvalidOperationException>(() => registerResponse.Validate());
		}

		[Test]
		public void Validate_ClientDataOriginMissing_Throws()
		{
			var registerResponse = CreateGoodRegisterResponse();
			registerResponse.ClientData.Origin = "";

			Assert.Throws<InvalidOperationException>(() => registerResponse.Validate());
		}

		[Test]
		public void Validate_ClientDataTypeMissing_Throws()
		{
			var registerResponse = CreateGoodRegisterResponse();
			registerResponse.ClientData.Type = "";

			Assert.Throws<InvalidOperationException>(() => registerResponse.Validate());
		}

		internal static FidoRegisterResponse CreateGoodRegisterResponse()
		{
			return new FidoRegisterResponse
			{
				RegistrationData = FidoRegistrationData.FromWebSafeBase64(TestVectors.RegistrationResponseDataBase64),
				ClientData = new FidoClientData
				{
					Challenge = TestVectors.ServerChallengeRegisterBase64,
					Origin = "http://localhost",
					Type = "type"
				}
			};
		}
    }
}
