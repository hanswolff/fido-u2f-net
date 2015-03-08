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
using System.Linq;
using FidoU2f.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FidoU2f
{
	public class FidoDeviceRegistrationSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var deviceRegistration = (FidoDeviceRegistration)value;

			writer.WriteStartObject();

			writer.WritePropertyName("Certificate");
			serializer.Serialize(writer, deviceRegistration.Certificate.ToWebSafeBase64());

			writer.WritePropertyName("Counter");
			serializer.Serialize(writer, deviceRegistration.Counter);

			writer.WritePropertyName("KeyHandle");
			serializer.Serialize(writer, deviceRegistration.KeyHandle.ToWebSafeBase64());

			writer.WritePropertyName("PublicKey");
			serializer.Serialize(writer, deviceRegistration.PublicKey.ToWebSafeBase64());

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var properties = jsonObject.Properties().ToLookup(x => x.Name.ToLowerInvariant());

			var serializedCertificate = properties["certificate"].Single().Value.ToString();
			var serializedPublicKey = properties["publickey"].Single().Value.ToString();
			var serializedKeyHandle = properties["keyhandle"].Single().Value.ToString();
			var serializedCounter = properties["counter"].Single().Value.ToString();

			return new FidoDeviceRegistration(
				FidoKeyHandle.FromWebSafeBase64(serializedKeyHandle),
				FidoPublicKey.FromWebSafeBase64(serializedPublicKey),
				FidoAttestationCertificate.FromWebSafeBase64(serializedCertificate),
				UInt32.Parse(serializedCounter));
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(FidoDeviceRegistration).IsAssignableFrom(objectType);
		}
	}
}
