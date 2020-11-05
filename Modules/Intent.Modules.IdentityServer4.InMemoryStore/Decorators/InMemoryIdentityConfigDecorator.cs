using Intent.Modules.Common.IdentityServer4.Decorators;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryIdentityConfigDecorator : IdentityConfigDecorator
    {
        public const string Identifier = "IdentityServer4.InMemoryStore.IdentityConfigDecorator";

        public override int Priority => 1;

        public override EntityCollection GetClients()
        {
            return new EntityCollection()
            {
                new Entity
                {
                    { "ClientId", @"$""{ApiResourceName}_pwd_client""" } ,
                    { "AllowedGrantTypes", "GrantTypes.ResourceOwnerPassword"},
                    { "RequireClientSecret", "false"},
                    { "AllowedScopes", @"{ ""openid"", ""profile"", ""email"", ApiResourceName }"}
                }
            };
        }
    }
}
