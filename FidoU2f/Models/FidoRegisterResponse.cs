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
using Newtonsoft.Json;

namespace FidoU2f.Models
{
	public class FidoRegisterResponse : IValidate
	{
		[JsonProperty("registrationData")]
		public string RegistrationDataBase64 { get; set; }

		[JsonProperty("clientData")]
		public string ClientDataBase64 { get; set; }

		private FidoRegistrationData _overrideRegistrationData;
		[JsonIgnore]
		public FidoRegistrationData RegistrationData
		{
			get { return _overrideRegistrationData ?? FidoRegistrationData.FromWebSafeBase64(RegistrationDataBase64); }
			set { _overrideRegistrationData = value; }
		}

		private FidoClientData _overrideClientData;
		[JsonIgnore]
		public FidoClientData ClientData
		{
			get { return _overrideClientData ?? FidoClientData.FromWebSafeBase64(ClientDataBase64); }
			set { _overrideClientData = value; }
		}

		public static FidoRegisterResponse FromJson(string json)
		{
			return JsonConvert.DeserializeObject<FidoRegisterResponse>(json);
		}

		public void Validate()
		{
			if (String.IsNullOrEmpty(RegistrationDataBase64))
				throw new InvalidOperationException("Registration data is missing in registration response");

			ClientData.Validate();
		}

		public string ToJson()
		{
			if (_overrideRegistrationData != null)
				RegistrationDataBase64 = JsonConvert.SerializeObject(_overrideRegistrationData);

			if (_overrideClientData != null)
				ClientDataBase64 = JsonConvert.SerializeObject(_overrideClientData);

            return JsonConvert.SerializeObject(this);
		}
	}
}
