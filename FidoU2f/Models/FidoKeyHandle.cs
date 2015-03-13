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
    [JsonConverter(typeof(FidoKeyHandleConverter))]
	public class FidoKeyHandle : IEquatable<FidoKeyHandle>
	{
		private readonly byte[] _bytes;

		public FidoKeyHandle(byte[] keyHandleBytes)
		{
			if (keyHandleBytes == null) throw new ArgumentNullException("keyHandleBytes");

			_bytes = keyHandleBytes;
		}

		public static FidoKeyHandle FromWebSafeBase64(string keyHandle)
		{
			if (keyHandle == null) throw new ArgumentNullException("keyHandle");

			return new FidoKeyHandle(WebSafeBase64Converter.FromBase64String(keyHandle));
		}

		public byte[] ToByteArray()
		{
			return _bytes;
		}

		public bool Equals(FidoKeyHandle other)
		{
			if (other == null) return false;
			return ToWebSafeBase64() == other.ToWebSafeBase64();
		}

		public string ToWebSafeBase64()
		{
			return WebSafeBase64Converter.ToBase64String(_bytes);
		}

		public override string ToString()
		{
			return ToWebSafeBase64();
		}
	}
}
