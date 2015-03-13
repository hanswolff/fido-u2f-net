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

using FidoU2f.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FidoU2f.Tests.Serializers
{
    [TestFixture]
    public class TestFidoAppIdSerializer
    {
        [Test]
        public void SerializeObject()
        {
            var fidoAppId = new FidoAppId("http://example.com");
            var serialized = JsonConvert.SerializeObject(fidoAppId);

            Assert.AreEqual("\"http://example.com\"", serialized);
        }

        [Test]
        public void DeserializeObject()
        {
            var deserialized = JsonConvert.DeserializeObject<FidoAppId>("\"http://example.com\"");

            Assert.AreEqual(new FidoAppId("http://example.com"), deserialized);
        }
    }
}
