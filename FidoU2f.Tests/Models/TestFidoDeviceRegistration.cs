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

using System.Text;
using FidoU2f.Models;
using NUnit.Framework;

namespace FidoU2f.Tests.Models
{
	[TestFixture]
	public class TestFidoDeviceRegistration
	{
		[Test]
		public void Serialize()
		{
			var keyHandle = new FidoKeyHandle(Encoding.Default.GetBytes("keyhandle"));
			var publicKey = new FidoPublicKey(Encoding.Default.GetBytes("publickey"));
			var certificate = new FidoAttestationCertificate(Encoding.Default.GetBytes("certificate"));

			var deviceRegistration = new FidoDeviceRegistration(keyHandle, publicKey, certificate, 12345);

			var serialized = deviceRegistration.Serialize();
			Assert.AreEqual("{\"Certificate\":\"Y2VydGlmaWNhdGU\",\"Counter\":12345,\"KeyHandle\":\"a2V5aGFuZGxl\",\"PublicKey\":\"cHVibGlja2V5\"}", serialized);
		}

		[Test]
		public void Deserialize()
		{
			var deviceRegistration = FidoDeviceRegistration.Deserialize("{\"Certificate\":\"Y2VydGlmaWNhdGU\",\"Counter\":12345,\"KeyHandle\":\"a2V5aGFuZGxl\",\"PublicKey\":\"cHVibGlja2V5\"}");

			var keyHandle = new FidoKeyHandle(Encoding.Default.GetBytes("keyhandle"));
			var publicKey = new FidoPublicKey(Encoding.Default.GetBytes("publickey"));
			var certificate = new FidoAttestationCertificate(Encoding.Default.GetBytes("certificate"));

			Assert.AreEqual(12345, deviceRegistration.Counter);
			Assert.IsTrue(certificate.Equals(deviceRegistration.Certificate));
			Assert.IsTrue(publicKey.Equals(deviceRegistration.PublicKey));
			Assert.IsTrue(keyHandle.Equals(deviceRegistration.KeyHandle));
		}

		[Test]
		public void Equals_Null()
		{
			var registration = CreateTestDeviceRegistration();
			Assert.IsFalse(registration.Equals(null));
		}

		[Test]
		public void Equals_AreEqual()
		{
			var registration1 = CreateTestDeviceRegistration();
			var registration2 = CreateTestDeviceRegistration();

			Assert.IsTrue(registration1.Equals(registration2));
		}

		[Test]
		public void Equals_DifferentCertificate()
		{
			var registration1 = CreateTestDeviceRegistration();

			var registration2 = new FidoDeviceRegistration(
				registration1.KeyHandle,
				registration1.PublicKey,
				new FidoAttestationCertificate(Encoding.UTF8.GetBytes("different certificate")),
				registration1.Counter);

			Assert.IsFalse(registration1.Equals(registration2));
		}

		[Test]
		public void Equals_DifferentKeyHandle()
		{
			var registration1 = CreateTestDeviceRegistration();

			var registration2 = new FidoDeviceRegistration(
				new FidoKeyHandle(Encoding.Default.GetBytes("different keyhandle")),
				registration1.PublicKey,
				registration1.Certificate,
				registration1.Counter);

			Assert.IsFalse(registration1.Equals(registration2));
		}

		[Test]
		public void Equals_DifferentPublicKey()
		{
			var registration1 = CreateTestDeviceRegistration();

			var registration2 = new FidoDeviceRegistration(
				registration1.KeyHandle,
				new FidoPublicKey(Encoding.Default.GetBytes("different publickey")),
				registration1.Certificate,
				registration1.Counter);

			Assert.IsFalse(registration1.Equals(registration2));
		}

		private static FidoDeviceRegistration CreateTestDeviceRegistration()
		{
			var keyHandle = new FidoKeyHandle(Encoding.Default.GetBytes("keyhandle"));
			var publicKey = new FidoPublicKey(Encoding.Default.GetBytes("publickey"));
			var certificate = new FidoAttestationCertificate(Encoding.Default.GetBytes("certificate"));

			var deviceRegistration = new FidoDeviceRegistration(keyHandle, publicKey, certificate, 12345);
			return deviceRegistration;
		}
	}
}
