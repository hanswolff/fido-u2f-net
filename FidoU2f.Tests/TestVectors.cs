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

namespace FidoU2f.Tests
{
	/// <summary>
	/// Test vectors from FIDO U2F: Raw Message Formats
	/// </summary>
	/// <remarks>
	/// http://fidoalliance.org/specs/fido-u2f-v1.0-ps-20141009/fido-u2f-raw-message-formats-ps-20141009.html
	/// </remarks>
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

		public const string SignResponseDataBase64 =
			"AQAAAAEwRAIgS18M0XU0zt2MNO4JVw71QqNT30Q2AwzkPUBt6HC4R3gCICZ7uZj6ybcmbrYOfLC16r39W6lhT1PHsiJy7BAEepI_";

		public static readonly string AttestationCertificate =
			WebSafeBase64Converter.ToBase64String(
				StringToByteArray(
					"3082013c3081e4a003020102020a47901280001155957352300a06082a8648ce3d0403023017311530130603550403130c476e756262792050696c6f74301e170d3132303831343138323933325a170d3133303831343138323933325a3031312f302d0603550403132650696c6f74476e756262792d302e342e312d34373930313238303030313135353935373335323059301306072a8648ce3d020106082a8648ce3d030107034200048d617e65c9508e64bcc5673ac82a6799da3c1446682c258c463fffdf58dfd2fa3e6c378b53d795c4a4dffb4199edd7862f23abaf0203b4b8911ba0569994e101300a06082a8648ce3d0403020347003044022060cdb6061e9c22262d1aac1d96d8c70829b2366531dda268832cb836bcd30dfa0220631b1459f09e6330055722c8d89b7f48883b9089b88d60d1d9795902b30410df"));

		public static readonly string KeyHandle =
			WebSafeBase64Converter.ToBase64String(
				StringToByteArray(
					"2a552dfdb7477ed65fd84133f86196010b2215b57da75d315b7b9e8fe2e3925a6019551bab61d16591659cbaf00b4950f7abfe6660e2e006f76868b772d70c25"));

		public static readonly string PublicKey =
			WebSafeBase64Converter.ToBase64String(
				StringToByteArray(
					"04b174bc49c7ca254b70d2e5c207cee9cf174820ebd77ea3c65508c26da51b657c1cc6b952f8621697936482da0a6d3d3826a59095daf6cd7c03e2e60385d2f6d9"));

		public static readonly byte[] AuthenticateResponse =
			StringToByteArray("0100000001304402204b5f0cd17534cedd8c34ee09570ef542a353df4436030ce43d406de870b847780220267bb998fac9b7266eb60e7cb0b5eabdfd5ba9614f53c7b22272ec10047a923f");

		static byte[] StringToByteArray(string hex)
		{
			var numberChars = hex.Length;
			var bytes = new byte[numberChars / 2];
			for (var i = 0; i < numberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}
	}
}
