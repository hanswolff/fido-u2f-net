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
                Challenge = startedRegistration.Challenge,
                UserName = GetCurrentUser()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterNewDeviceViewModel model)
        {
            model = model ?? new RegisterNewDeviceViewModel();

            if (!String.IsNullOrEmpty(model.RawRegisterResponse))
            {
                var u2f = new FidoUniversalTwoFactor();

                var challenge = model.Challenge;
                var startedRegistration = GetFidoRepository().GetStartedRegistration(GetCurrentUser(), challenge);

                var deviceRegistration = u2f.FinishRegistration(startedRegistration, model.RawRegisterResponse, GetTrustedDomains());
                GetFidoRepository().StoreDeviceRegistration(GetCurrentUser(), deviceRegistration);
                GetFidoRepository().RemoveStartedRegistration(GetCurrentUser(), model.Challenge);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Login(string keyHandle)
        {
            var model = new LoginDeviceViewModel { KeyHandle = keyHandle };

            try
            {
                var u2f = new FidoUniversalTwoFactor();
                var appId = new FidoAppId(Request.Url);

                var deviceRegistration = GetFidoRepository().GetDeviceRegistrationsOfUser(GetCurrentUser()).FirstOrDefault(x => x.KeyHandle.ToWebSafeBase64() == keyHandle);
                if (deviceRegistration == null)
                {
                    ModelState.AddModelError("", "Unknown key handle: " + keyHandle);
                    return View(model);
                }

                var startedRegistration = u2f.StartAuthentication(appId, deviceRegistration);

                model = new LoginDeviceViewModel
                {
                    AppId = startedRegistration.AppId.ToString(),
                    Challenge = startedRegistration.Challenge,
                    KeyHandle = startedRegistration.KeyHandle.ToWebSafeBase64(),
                    UserName = GetCurrentUser()
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetType().Name + ": " + ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginDeviceViewModel model)
        {
            model = model ?? new LoginDeviceViewModel();

            try
            {
                if (!String.IsNullOrEmpty(model.RawAuthenticationResponse))
                {
                    var u2f = new FidoUniversalTwoFactor();
                    var appId = new FidoAppId(Request.Url);

                    var deviceRegistration = GetFidoRepository().GetDeviceRegistrationsOfUser(GetCurrentUser()).FirstOrDefault(x => x.KeyHandle.ToWebSafeBase64() == model.KeyHandle);
                    if (deviceRegistration == null)
                    {
                        ModelState.AddModelError("", "Unknown key handle: " + model.KeyHandle);
                        return View(new LoginDeviceViewModel());
                    }

                    var challenge = model.Challenge;

                    var startedAuthentication = new FidoStartedAuthentication(appId, challenge,
                        FidoKeyHandle.FromWebSafeBase64(model.KeyHandle ?? ""));

                    var counter = u2f.FinishAuthentication(startedAuthentication, model.RawAuthenticationResponse, deviceRegistration, GetTrustedDomains());

                    // save the counter somewhere, the device registration of the next authentication should use this updated counter
                    deviceRegistration.UpdateCounter(counter);

                    return RedirectToAction("LoginSuccess");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetType().Name + ": " + ex.Message);
            }

            return View(model);
        }

        public ActionResult LoginSuccess()
        {
            return View();
        }

        private FidoFacetId[] GetTrustedDomains()
        {
            return new[] { new FidoFacetId(Request.Url) };
        }
    }
}