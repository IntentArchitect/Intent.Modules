using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.Selfhost
{
    static class NugetPackages
    {
        public static readonly INugetPackageInfo IdentityServer4 = new NugetPackageInfo("IdentityServer4", "4.1.0");
    }
}
