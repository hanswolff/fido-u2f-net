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

using NUnit.Framework;

namespace FidoU2f.Tests
{
	[TestFixture]
	public class TestWebSafeBase64Converter
	{
		[TestCase(null, null)]
		[TestCase(new byte[] { }, "")]
		[TestCase(new byte[] { 0 }, "AA")]
		[TestCase(new byte[] { 1, 2 }, "AQI")]
		[TestCase(new byte[] { 3, 4, 5 }, "AwQF")]
		[TestCase(new byte[] { 6, 7, 8, 9 }, "BgcICQ")]
		[TestCase(new byte[] { 32 }, "IA")]
		[TestCase(new byte[] { 32, 33 }, "ICE")]
		[TestCase(new byte[] { 32, 33, 34 }, "ICEi")]
		[TestCase(new byte[] { 32, 33, 34, 35 }, "ICEiIw")]
		public void ToBase64String(byte[] input, string output)
		{
			Assert.AreEqual(output, WebSafeBase64Converter.ToBase64String(input));
		}

		[TestCase(null, null)]
		[TestCase("", new byte[] { })]
		[TestCase("AA", new byte[] { 0 })]
		[TestCase("AQI", new byte[] { 1, 2 })]
		[TestCase("AwQF", new byte[] { 3, 4, 5 })]
		[TestCase("BgcICQ", new byte[] { 6, 7, 8, 9 })]
		[TestCase("IA", new byte[] { 32 })]
		[TestCase("ICE", new byte[] { 32, 33 })]
		[TestCase("ICEi", new byte[] { 32, 33, 34 })]
		[TestCase("ICEiIw", new byte[] { 32, 33, 34, 35 })]
		public void FromBase64String(string input, byte[] output)
		{
			Assert.AreEqual(output, WebSafeBase64Converter.FromBase64String(input));
		}
	}
}
