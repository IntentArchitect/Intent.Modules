using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup;
using System;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer4.Decorators
{
    public class IdentityServer4CoreWebStartupDecorator : CoreWebStartupDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.IdentityServer4.CoreWebStartupDecorator";

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] { NugetPackages.IdentityServer4 };
        }
    }
}
