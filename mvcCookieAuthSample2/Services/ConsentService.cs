using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;

        public readonly IResourceStore _resourceStore;

        public readonly IIdentityServerInteractionService _identityServerInteractionService;

        public ConsentService(IClientStore clientStore, IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }




        #region 私有方法
        private ConsentViewModel CreatConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel model)
        {
            var remberConsent = model?.RemberConsent??true;
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RemberConsent = remberConsent;
            vm.IdentityScopes = resources.IdentityResources.Select(i => CreatScopeViewModel(i, selectedScopes.Contains(i.Name)||model==null));

            vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreatScopeViewModel(x, selectedScopes.Contains(x.Name) || model == null));
            return vm;
        }

        private ScopeViewModel CreatScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = check||scope.Required,
                Required = scope.Required,
                Emphasize = scope.Emphasize,


            };
        }

        private ScopeViewModel CreatScopeViewModel(IdentityResource identityResource,bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = check||identityResource.Required,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize,


            };
        }

        #endregion
        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl, InputConsentViewModel model=null)
        {
           
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);

            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreatConsentViewModel(request, client, resources, model);
            vm.ReturnUrl = returnUrl;
            return vm;

        }

        public async Task<ProcessConsentResult> PorcessContenst(InputConsentViewModel model)
        {
            ConsentResponse constentRespone = null;
            var result = new ProcessConsentResult();

            if (model.Button == "no")
            {
                constentRespone = ConsentResponse.Denied;
            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    constentRespone = new ConsentResponse()
                    {
                        ScopesConsented = model.ScopesConsented,
                        RememberConsent = model.RemberConsent,
                    };
                }

                result.ValidationError = "请至少选择一个";
            }

            if (constentRespone != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, constentRespone);
                result.RedirectUrl=model.ReturnUrl;


            }
            else
            {
                var consentViewModel =await BuildConsentViewModel(model.ReturnUrl, model);
                result.ViewModel = consentViewModel;
            }

            return result;
        }

    }
}
