using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Entities
{
    public static class NugetPackages
    {
        public static INugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "0.1.4-beta", "net45");
    }
}
