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
using Org.BouncyCastle.Crypto.Digests;

namespace FidoU2f
{
    static class Helpers
    {
        public static byte[] Sha256(string text)
        {
            var bytes = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);

            var sha256 = new Sha256Digest();
            var hash = new byte[sha256.GetDigestSize()];
            sha256.BlockUpdate(bytes, 0, bytes.Length);
            sha256.DoFinal(hash, 0);
            return hash;
        }

        public static string GetAuthority(Uri uri)
        {
            var isDefaultPort =
                (uri.Scheme == "http" && uri.Port == 80) ||
                (uri.Scheme == "https" && uri.Port == 443);

            return uri.Scheme + "://" + uri.DnsSafeHost + (isDefaultPort ? "" : ":" + uri.Port);
        }
    }
}
