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
	public class TestFidoRegistrationData
	{
		[Test]
		public void FromWebSafeBase64()
		{
			var registrationData = FidoRegistrationData.FromWebSafeBase64(TestVectors.RegistrationResponseDataBase64);

			Assert.IsNotNull(registrationData.AttestationCertificate);
			Assert.IsNotNullOrEmpty(registrationData.KeyHandle.ToString());
			Assert.IsNotNullOrEmpty(registrationData.Signature.ToString());
			Assert.IsNotNullOrEmpty(registrationData.UserPublicKey.ToString());
		}

        [Test]
	    public void Validate_Good_NoException()
        {
            var value = CreateGoodRegistrationData();
            value.Validate();
        }

        [Test]
        public void Validate_AttestationCertificateMissing_Throws()
        {
            var value = CreateGoodRegistrationData();
            value.AttestationCertificate = null;

            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }

        [Test]
        public void Validate_KeyHandleMissing_Throws()
        {
            var value = CreateGoodRegistrationData();
            value.KeyHandle = null;

            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }

        [Test]
        public void Validate_SignatureMissing_Throws()
        {
            var value = CreateGoodRegistrationData();
            value.Signature = null;

            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }

        [Test]
        public void Validate_UserPublicKeyMissing_Throws()
        {
            var value = CreateGoodRegistrationData();
            value.UserPublicKey = null;

            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }

        private FidoRegistrationData CreateGoodRegistrationData()
	    {
            return FidoRegistrationData.FromWebSafeBase64(TestVectors.RegistrationResponseDataBase64);
        }
	}
}
