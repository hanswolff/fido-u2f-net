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

namespace FidoU2f
{
	/// <summary>
	/// BASE64 converter that uses only web-safe characters
	/// </summary>
	public static class WebSafeBase64Converter
	{
		/// <summary>
		/// Converts a byte array to a web-safe base64 string
		/// </summary>
		/// <param name="byteArray">byte array to convert</param>
		/// <returns>web safe base64 encoded string</returns>
		public static string ToBase64String(byte[] byteArray)
		{
			if (byteArray == null) return null;

			var result = Convert.ToBase64String(byteArray)
				.TrimEnd('=')
				.Replace('+', '-')
				.Replace('/', '_');
			return result;
		}

		/// <summary>
		/// Converts a byte array to a web-safe base64 string
		/// </summary>
		/// <param name="value">string value to convert</param>
		/// <returns>web safe base64 encoded string</returns>
		public static string ToBase64String(string value)
		{
			if (value == null) return null;

			var byteArray = Encoding.UTF8.GetBytes(value);
			return ToBase64String(byteArray);
		}

		/// <summary>
		/// Decode a web-safe base64 string to a byte array
		/// </summary>
		/// <param name="webSafeBase64">web safe base64 encoded string</param>
		/// <returns>byte array</returns>
		public static byte[] FromBase64String(string webSafeBase64)
		{
			if (webSafeBase64 == null) return null;

			webSafeBase64 = webSafeBase64
				.Trim()
				.Replace('-', '+')
				.Replace('_', '/');

			var mod4 = webSafeBase64.Length % 4;
			if (mod4 > 0)
			{
				webSafeBase64 += new string('=', 4 - mod4);
			}

			return Convert.FromBase64String(webSafeBase64);
		}
	}
}
