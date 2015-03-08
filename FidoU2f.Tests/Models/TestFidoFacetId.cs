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
	public class TestFidoFacetId
	{
		[TestCase("http://localhost")]
		[TestCase("http://localhost/")]
		[TestCase("http://localhost:12345")]
		[TestCase("http://localhost:12345/")]
		[TestCase("http://localhost/somepath")]
		[TestCase("https://www.website.com")]
		[TestCase("https://www.website.com///")]
		public void Constructor_WellFormattedFacetIds(string facetId)
		{
			new FidoFacetId(facetId);
		}

		[TestCase("ftp://localhost")]
		[TestCase("http://localhost:9999999")]
		[TestCase("/path")]
		public void Constructor_IncorrectlyFormattedFacetIds(string facetId)
		{
			Assert.Throws<FormatException>(() => new FidoFacetId(facetId));
		}

		[TestCase("ftp://localhost")]
		[TestCase("/path")]
		public void Constructor_IncorrectUri(string facetId)
		{
			Assert.Throws<FormatException>(() => new FidoFacetId(new Uri(facetId, UriKind.RelativeOrAbsolute)));
		}

		[Test]
		public void Equals_Null()
		{
			Assert.IsFalse(new FidoFacetId("http://example.com").Equals((FidoFacetId)null));
		}

		[TestCase("http://example.com", "http://example.com")]
		[TestCase("http://Example.com", "http://example.com")]
		[TestCase("http://example.com/", "http://example.com")]
		[TestCase("http://example.com/path", "http://example.com")]
		[TestCase("http://example.com/path", "http://example.com")]
		[TestCase("http://example.com", "http://example.com/path")]
		public void Equals_AppId(string facetId1, string appId)
		{
			Assert.IsTrue(new FidoFacetId(facetId1).Equals(new FidoAppId(appId)));
			Assert.IsTrue(new FidoFacetId(new Uri(facetId1)).Equals(new FidoAppId(appId)));
		}

		[TestCase("http://example.com", "http://example.com")]
		[TestCase("http://Example.com", "http://example.com")]
		[TestCase("http://example.com/", "http://example.com")]
		[TestCase("http://example.com/path", "http://example.com")]
		[TestCase("http://example.com", "http://example.com/path")]
		public void Equals_FacetId(string facetId1, string facetId2)
		{
			Assert.IsTrue(new FidoFacetId(facetId1).Equals(new FidoFacetId(facetId2)));
		}

		[Test]
		public void ToString_Works()
		{
			Assert.AreEqual("http://localhost", new FidoFacetId("http://localhost/").ToString());
		}
	}
}
