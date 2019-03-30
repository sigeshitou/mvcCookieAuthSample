using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvcCookieAuthSample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using mvcCookieAuthSample.Services;

namespace mvcCookieAuthSample.Controllers
{
    public class ConsentController:Controller
    {

        public readonly ConsentService _consentService;

        public ConsentController(ConsentService consentService )
        {
            _consentService = consentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model =await _consentService.BuildConsentViewModel(returnUrl);
            if (model == null)
            {
                
            }
           
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result = await _consentService.PorcessContenst(viewModel);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }


            return View(result.ViewModel);
        }

    }
}
