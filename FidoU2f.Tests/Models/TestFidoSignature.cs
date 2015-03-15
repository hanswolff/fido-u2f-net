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
	public class TestFidoSignature
	{
        [Test]
        public void Constructor()
        {
            var value = new FidoSignature(Encoding.Default.GetBytes("signature"));
            Assert.AreEqual("signature", Encoding.Default.GetString(value.ToByteArray()));
        }

        [Test]
        public void Constructor_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FidoSignature(null));
        }

        [Test]
		public void Equals_Null()
		{
			Assert.IsFalse(FidoSignature.FromWebSafeBase64("").Equals(null));
		}

		[Test]
		public void Equals_AreEqual()
		{
			var value1 = new FidoSignature(Encoding.Default.GetBytes("signature"));
			var value2 = new FidoSignature(Encoding.Default.GetBytes("signature"));
			Assert.IsTrue(value1.Equals(value2));
		}

		[Test]
		public void Equals_NotEqual()
		{
			var value1 = new FidoSignature(Encoding.Default.GetBytes("signature"));
			var value2 = new FidoSignature(Encoding.Default.GetBytes("Signature"));
			Assert.IsFalse(value1.Equals(value2));
		}

        [Test]
        public void Validate_Good_NoException()
        {
            var value = new FidoSignature(WebSafeBase64Converter.FromBase64String(TestVectors.PublicKeyBase64));
            value.Validate();
        }

        [Test]
        public void Validate_BytesEmpty_Throws()
        {
            var value = new FidoSignature(new byte[0]);
            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }
    }
}
