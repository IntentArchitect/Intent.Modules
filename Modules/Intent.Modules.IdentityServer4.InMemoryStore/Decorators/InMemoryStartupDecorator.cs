using Intent.Modules.Common.IdentityServer4.Decorators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryStartupDecorator : StartupDecorator
    {
        public const string Identifier = "IdentityServer4.InMemoryStore.StartupDecorator";

        public override int Priority => 1;

        public override IReadOnlyCollection<string> GetServicesConfigurationStatements()
        {
            return new[] 
            {
                "AddTestUsers(TestUsers.Users)"
            };
        }
    }
}
