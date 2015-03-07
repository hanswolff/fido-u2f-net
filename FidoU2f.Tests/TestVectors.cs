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

namespace FidoU2f.Tests
{
	/// <summary>
	/// Test vectors from FIDO U2F: Raw Message Formats (fido-u2f-v1.0-rd-20140209 REVIEW DRAFT)
	/// </summary>
	public static class TestVectors
	{
		public const string AppIdEnroll = "http://example.com";
		public const string Origin = "http://example.com";
		public static readonly FidoFacetId[] TrustedDomains = { new FidoFacetId("http://example.com") };

		public const string ServerChallengeRegisterBase64 = "vqrS6WXDe1JUs5_c3i4-LkKIHRr-3XVb3azuA5TifHo";

		public const string ServerChallengeAuthBase64 = "opsXqUifDriAAmWclinfbS0e-USY0CgyJHe_Otd7z8o";

		public const string ChannelIdJson =
			"{\"kty\":\"EC\",\"crv\":\"P-256\",\"x\":\"HzQwlfXX7Q4S5MtCCnZUNBw3RMzPO9tOyWjBqRl4tJ8\",\"y\":\"XVguGFLIZx1fXg3wNqfdbn75hi4-_7-BxhMljw42Ht4\"}";

		public const string ClientDataRegister = "{\"typ\":\"navigator.id.finishEnrollment\"," + "\"challenge\":\"" + ServerChallengeRegisterBase64
			+ "\",\"cid_pubkey\":" + ChannelIdJson + ",\"origin\":\"" + Origin + "\"}";

		public const string ClientDataAuth = "{\"typ\":\"navigator.id.getAssertion\"," + "\"challenge\":\"" + ServerChallengeAuthBase64
			+ "\",\"cid_pubkey\":" + ChannelIdJson + ",\"origin\":\"" + Origin + "\"}";

		public const string RegistrationResponseDataBase64 =
			"BQSxdLxJx8olS3DS5cIHzunPF0gg69d+o8ZVCMJtpRtlfBzGuVL4YhaXk2SC2gptPTgmpZCV2vbNfAPi5gOF0vbZQCpVLf23R37WX9hBM/hhlgELIhW1faddMVt7no/i45JaYBlVG6th0WWRZZy68AtJUPer/mZg4uAG92hot3LXDCUwggE8MIHkoAMCAQICCkeQEoAAEVWVc1IwCgYIKoZIzj0EAwIwFzEVMBMGA1UEAxMMR251YmJ5IFBpbG90MB4XDTEyMDgxNDE4MjkzMloXDTEzMDgxNDE4MjkzMlowMTEvMC0GA1UEAxMmUGlsb3RHbnViYnktMC40LjEtNDc5MDEyODAwMDExNTU5NTczNTIwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAASNYX5lyVCOZLzFZzrIKmeZ2jwURmgsJYxGP//fWN/S+j5sN4tT15XEpN/7QZnt14YvI6uvAgO0uJEboFaZlOEBMAoGCCqGSM49BAMCA0cAMEQCIGDNtgYenCImLRqsHZbYxwgpsjZlMd2iaIMsuDa80w36AiBjGxRZ8J5jMAVXIsjYm39IiDuQibiNYNHZeVkCswQQ3zBFAiAUcYmbzDmH5i6CAsmznDPBkDP3NANS26gPyrAX25Iw5AIhAIJnfWc9iRkzreb2F+Xb3i4kfnBCP9WteASm09OWHvhx";
	}
}
