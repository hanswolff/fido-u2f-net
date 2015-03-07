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
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace FidoU2f.Models
{
	public class FidoPublicKey : IEquatable<FidoPublicKey>
	{
		private readonly byte[] _bytes;

		public FidoPublicKey(byte[] publicKeyBytes)
		{
			if (publicKeyBytes == null) throw new ArgumentNullException("publicKeyBytes");

			_bytes = publicKeyBytes;
		}

		public ICipherParameters PublicKey
		{
			get
			{
				var curve = SecNamedCurves.GetByOid(SecObjectIdentifiers.SecP256r1);
				var point = curve.Curve.DecodePoint(_bytes);
				var ecP = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

				return new ECPublicKeyParameters(point, ecP);
			}
		}

		public static FidoPublicKey FromWebSafeBase64(string publicKey)
		{
			if (publicKey == null) throw new ArgumentNullException("publicKey");

			return new FidoPublicKey(WebSafeBase64Converter.FromBase64String(publicKey));
		}

		public byte[] ToByteArray()
		{
			return _bytes;
		}

		public bool Equals(FidoPublicKey other)
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
