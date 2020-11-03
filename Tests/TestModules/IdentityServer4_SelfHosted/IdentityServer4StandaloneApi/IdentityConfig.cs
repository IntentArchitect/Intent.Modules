using System;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("IdentityServer4.Selfhost.IdentityConfig", Version = "1.0")]

namespace IdentityServer4StandaloneApi
{
    public class IdentityConfig
    {
        public const string ApiResourceName = "api";

        public static IEnumerable<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = ApiResourceName + "_client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RequireConsent = false,
                RedirectUris = { },
                PostLogoutRedirectUris = {  },
                AllowedScopes = { "openid", "profile", "email", ApiResourceName },
                AllowedCorsOrigins = {  },
                AllowAccessTokensViaBrowser = true,
               
            },
        };

        public static IEnumerable<ApiScope> Scopes = new List<ApiScope>
        {
            new ApiScope(ApiResourceName)
        };

        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

        public static IEnumerable<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource(ApiResourceName)
        };
    }
}