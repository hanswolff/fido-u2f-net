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
using FidoU2f.Serializers;
using Newtonsoft.Json;

namespace FidoU2f.Models
{
	/// <summary>
	/// FIDO AppId (see section 3 in FIDO specification for valid FacetIds)
	/// </summary>
	[JsonConverter(typeof(FidoAppIdConverter))]
	public class FidoAppId : IEquatable<FidoAppId>, IEquatable<FidoFacetId>
	{
		private readonly Uri _appUri;

		public FidoAppId(Uri appId)
		{
			if (!appId.IsAbsoluteUri)
				ThrowFormatException();
			_appUri = appId;

			ValidateUri(appId);
		}

		public FidoAppId(string facetId)
		{
			if (!Uri.TryCreate(facetId, UriKind.Absolute, out _appUri))
				ThrowFormatException();

			ValidateUri(_appUri);
		}

		private void ValidateUri(Uri uri)
		{
			var scheme = uri.Scheme.ToLowerInvariant();
			if (scheme != "http" && scheme != "https")
				ThrowFormatException();
		}

		public bool Equals(FidoFacetId other)
		{
			if (other == null) return false;
			return ToString().StartsWith(other.ToString());
		}

		public bool Equals(FidoAppId other)
		{
			if (other == null) return false;
			var localAuthority = GetAuthority(_appUri);
			var otherAuthority = GetAuthority(other._appUri);
			return localAuthority == otherAuthority;
		}

		private static string GetAuthority(Uri uri)
		{
			var isDefaultPort = 
				(uri.Scheme == "http" && uri.Port == 80) || 
				(uri.Scheme == "https" && uri.Port == 443);

			return uri.Scheme + "://" + uri.DnsSafeHost + (isDefaultPort ? "" : ":" + uri.Port);
		}

		private static void ThrowFormatException()
		{
			throw new FormatException("FIDO App ID must be a URL prefix (e.g. 'https://website.com')");
		}

		public override string ToString()
		{
			return GetAuthority(_appUri);
		}
	}
}
