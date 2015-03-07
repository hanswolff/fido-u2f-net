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
	public class TestFidoAppId
	{
		[TestCase("http://localhost")]
		[TestCase("http://localhost/")]
		[TestCase("http://localhost:12345")]
		[TestCase("http://localhost:12345/")]
		[TestCase("https://www.website.com")]
		[TestCase("https://www.website.com///")]
		public void Constructor_WellFormattedAppIds(string appId)
		{
			new FidoAppId(appId);
		}

		[TestCase("ftp://localhost")]
		[TestCase("http://localhost:9999999")]
		[TestCase("http://localhost/somepath")]
		[TestCase("/path")]
		public void Constructor_IncorrectlyFormattedFacetIds(string appId)
		{
			Assert.Throws<FormatException>(() => new FidoAppId(appId));
		}

		[TestCase("ftp://localhost")]
		[TestCase("http://localhost/somepath")]
		[TestCase("/path")]
		public void Constructor_IncorrectUri(string appId)
		{
			Assert.Throws<FormatException>(() => new FidoAppId(new Uri(appId, UriKind.RelativeOrAbsolute)));
		}

		[Test]
		public void Equals_Null()
		{
			Assert.IsFalse(new FidoAppId("http://example.com").Equals((FidoAppId)null));
		}

		[TestCase("http://example.com", "http://example.com")]
		[TestCase("http://Example.com", "http://example.com")]
		[TestCase("http://example.com/", "http://example.com")]
		public void Equals(string appId1, string appId2)
		{
			Assert.IsTrue(new FidoAppId(appId1).Equals(new FidoAppId(appId2)));
			Assert.IsTrue(new FidoAppId(appId1).Equals(appId2));
			Assert.IsTrue(new FidoAppId(appId1).Equals(new FidoFacetId(appId2)));
		}

		[Test]
		public void ToString_Works()
		{
			Assert.AreEqual("http://localhost", new FidoAppId("http://localhost/").ToString());
		}
	}
}
