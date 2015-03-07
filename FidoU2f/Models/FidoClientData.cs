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
using Newtonsoft.Json.Linq;

namespace FidoU2f.Models
{
	public class FidoClientData
	{
		[JsonProperty("typ")]
		public string Type { get; set; }

		[JsonProperty("challenge")]
		public string Challenge { get; set; }

		[JsonProperty("origin")]
		public string Origin { get; set; }

		private string _overriddenRawJsonValue;
		[JsonIgnore]
		public string RawJsonValue
		{
			get { return _overriddenRawJsonValue ?? JsonConvert.SerializeObject(this); }
		}

		public static FidoClientData FromJson(string json)
		{
			var element = JObject.Parse(json);
			if (element == null)
				throw new InvalidOperationException("Client data must be in JSON format");

			JToken type, challenge, orgin;
			if (!element.TryGetValue("typ", out type))
				throw new InvalidOperationException("Client data is missing 'typ' param");
			if (!element.TryGetValue("challenge", out challenge))
				throw new InvalidOperationException("Client data is missing 'challenge' param");

			var clientData = new FidoClientData
			{
				_overriddenRawJsonValue = json,
				Type = type.ToString(),
				Challenge = challenge.ToString()
			};

			if (element.TryGetValue("origin", out orgin))
				clientData.Origin = orgin.ToString();

			return clientData;
		}

		public void Validate()
		{
			if (String.IsNullOrEmpty(Type))
				throw new InvalidOperationException("Type is missing in client data");

			if (String.IsNullOrEmpty(Challenge))
				throw new InvalidOperationException("Challenge is missing in client data");

			if (String.IsNullOrEmpty(Origin))
				throw new InvalidOperationException("Origin is missing in client data");
		}
	}
}
