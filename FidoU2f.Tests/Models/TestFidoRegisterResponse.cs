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
using NUnit.Framework;

namespace FidoU2f.Tests.Models
{
	[TestFixture]
	public class TestFidoRegisterResponse
	{
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
			registerResponse.RegistrationData = "";

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
				RegistrationData = "registration data",
				ClientData = new FidoClientData
				{
					Challenge = WebSafeBase64Converter.ToBase64String(Encoding.Default.GetBytes("random challenge")),
					Origin = "http://localhost",
					Type = "type"
				}
			};
		}
    }
}
