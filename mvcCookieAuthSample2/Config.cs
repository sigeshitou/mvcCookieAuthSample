using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace mvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResoure()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","API Aplication")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
                {
                    new Client()
                    {
                        ClientId="mvc",
                        ClientName="Mvc Client",
                        ClientUri="http://localhost:5001",
                        LogoUri="http://www.jjyc.org/Scripts/jjyc2017/kindeditor/attached/image/20161023/20161023210006_1848.png",
                        Description="MVC",
                        AllowOfflineAccess=true,
                        AlwaysIncludeUserClaimsInIdToken=true,
                        RequireConsent=true,
                        AllowAccessTokensViaBrowser=true,
                        //AllowedGrantTypes=GrantTypes.ClientCredentials,
                         AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                        ClientSecrets={new Secret("secret".Sha256())},
                        AllowedScopes=
                        {
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Email,
                        },
                        RedirectUris={"http://localhost:5001/signin-oidc"},
                        PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc" },

                    },

                };
        }
        //public static IEnumerable<IdentityResource> GetIdentityResources()
        //{
        //    return new List<IdentityResource>
        //    {
        //        new IdentityResources.OpenId(),
        //        new IdentityResources.Profile(),
        //    };
        //}
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>{

                 new IdentityResources.OpenId(),
                 new IdentityResources.Profile(),
                 new IdentityResources.Email(),
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="10000",
                    Username="sigeshitou",
                    
                    Password="sigeshitou"
                }
            };
        }
    }
}