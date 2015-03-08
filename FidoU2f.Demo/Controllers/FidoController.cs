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
using System.Web.Mvc;
using FidoU2f.Demo.Models;
using FidoU2f.Demo.Services;
using FidoU2f.Models;

namespace FidoU2f.Demo.Controllers
{
    public class FidoController : Controller
    {
		private static IFidoRepository _fidoRepository;

	    public static string GetCurrentUser()
	    {
			return ""; // users are ignored in this implementation
	    }

	    public static IFidoRepository GetFidoRepository()
	    {
		    return _fidoRepository ?? (_fidoRepository = new InMemoryFidoRepository());
	    }

        public ActionResult Index()
        {
	        var model = new RegistrationsViewModel
	        {
				StartedRegistrations = GetFidoRepository().GetAllStartedRegistrationsOfUser(GetCurrentUser()).ToList(),
				DeviceRegistrations = GetFidoRepository().GetDeviceRegistrationsOfUser(GetCurrentUser()).ToList()
	        };

            return View(model);
        }

		[HttpGet]
		public ActionResult Register()
		{
			var u2f = new FidoUniversalTwoFactor();
			var appId = new FidoAppId(Request.Url);
			var startedRegistration = u2f.StartRegistration(appId);

			GetFidoRepository().StoreStartedRegistration(GetCurrentUser(), startedRegistration);

			var model = new RegisterNewDeviceViewModel
			{
				AppId = startedRegistration.AppId.ToString(),
				Challenge = startedRegistration.Challenge
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult Register(RegisterNewDeviceViewModel model)
		{
			if (!String.IsNullOrEmpty(model.RawRegisterResponse))
			{
				var u2f = new FidoUniversalTwoFactor();
				var appId = new FidoAppId(Request.Url);

				var challenge = model.Challenge;
				var startedRegistration = GetFidoRepository().GetStartedRegistration(GetCurrentUser(), challenge);

				var deviceRegistration = u2f.FinishRegistration(startedRegistration, model.RawRegisterResponse, GetTrustedDomains());
				GetFidoRepository().StoreDeviceRegistration(GetCurrentUser(), deviceRegistration);
				GetFidoRepository().RemoveStartedRegistration(GetCurrentUser(), model.Challenge);

				return RedirectToAction("Index");
			}

			return View(model);
		}

	    private FidoFacetId[] GetTrustedDomains()
	    {
		    return new[] { new FidoFacetId(Request.Url) };
	    }
    }
}