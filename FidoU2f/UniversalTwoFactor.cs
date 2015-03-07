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
using System.Collections.Generic;
using System.Linq;
using FidoU2f.Models;

namespace FidoU2f
{
	public class UniversalTwoFactor
	{
		public static readonly string Version = "U2F_V2";

		public const string AuthenticateType = "navigator.id.getAssertion";
		public const string RegisterType = "navigator.id.finishEnrollment";


		private readonly IGenerateFidoChallenge _generateFidoChallenge;

		public UniversalTwoFactor()
			: this(null)
		{
		}

		public UniversalTwoFactor(IGenerateFidoChallenge generateFidoChallenge)
		{
			_generateFidoChallenge = generateFidoChallenge ?? new GenerateRandomFidoChallenge();
		}

		public FidoStartedRegistration StartRegistration(FidoAppId appId)
		{
			var challengeBytes = _generateFidoChallenge.GenerateChallenge();
			var challenge = WebSafeBase64Converter.ToBase64String(challengeBytes);

			return new FidoStartedRegistration(appId, challenge);
		}

		public FidoDeviceRegistration FinishRegistration(FidoStartedRegistration startedRegistration, 
			string deviceResponse, IEnumerable<FidoFacetId> trustedFacetIds)
		{
			if (deviceResponse == null) throw new ArgumentNullException("deviceResponse");

			var registerResponse = FidoRegisterResponse.FromJson(deviceResponse);
			return FinishRegistration(startedRegistration, registerResponse, trustedFacetIds);
		}

		public FidoDeviceRegistration FinishRegistration(FidoStartedRegistration startedRegistration, 
			FidoRegisterResponse registerResponse, IEnumerable<FidoFacetId> trustedFacetIds)
		{
			if (startedRegistration == null) throw new ArgumentNullException("startedRegistration");
			if (registerResponse == null) throw new ArgumentNullException("registerResponse");
			if (trustedFacetIds == null) throw new ArgumentNullException("trustedFacetIds");

			registerResponse.Validate();

			if (registerResponse.ClientData.Type != RegisterType)
			{
				var message = String.Format("Unexpected type in client data (expected '{0}' but was '{1}')",
					RegisterType, registerResponse.ClientData.Type);
				throw new InvalidOperationException(message);
			}

			if (registerResponse.ClientData.Challenge != startedRegistration.Challenge)
				throw new InvalidOperationException("Incorrect challenge signed in client data");

			var origin = registerResponse.ClientData.Origin;
            if (!trustedFacetIds.Any(x => x.ToString().Equals(origin)))
				throw new InvalidOperationException(String.Format("{0} is not a recognized trusted origin for this backend", origin));

			// TODO: create device registration

			throw new NotImplementedException();
		}
	}
}
