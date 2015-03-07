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

namespace FidoU2f.Models
{
	/// <summary>
	/// FIDO FacetId (see section 3 in FIDO specification for valid FacetIds)
	/// </summary>
	public class FidoFacetId
	{
		private readonly string _facetId;

		public FidoFacetId(Uri facetId)
		{
			if (!facetId.IsAbsoluteUri)
				ThrowFormatException();

			ValidateUri(facetId);

			_facetId = facetId.ToString().TrimEnd('/');
		}

		public FidoFacetId(string facetId)
		{
			Uri uri;
			if (!Uri.TryCreate(facetId, UriKind.Absolute, out uri))
				ThrowFormatException();

			ValidateUri(uri);

			_facetId = uri.ToString().TrimEnd('/');
		}

		private void ValidateUri(Uri uri)
		{
			var scheme = uri.Scheme.ToLowerInvariant();
			if (scheme != "http" && scheme != "https")
				ThrowFormatException();
		}

		private static void ThrowFormatException()
		{
			throw new FormatException("FIDO Facet ID must be a URL prefix (e.g. 'https://website.com')");
		}

		public override string ToString()
		{
			return _facetId;
		}
	}
}
