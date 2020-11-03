using System;
using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.IdentityServer4.Decorators
{
    public abstract class StartupDecorator : ITemplateDecorator
    {
        public abstract int Priority { get; }

        public abstract IReadOnlyCollection<string> GetServicesConfigurationStatements();
    }
}
