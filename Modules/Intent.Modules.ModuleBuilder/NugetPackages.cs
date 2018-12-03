using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentArchitectPackager = new NugetPackageInfo("Intent.IntentArchitectPackager", "1.2.2");
        public static NugetPackageInfo IntentMetadata = new NugetPackageInfo("Intent.SoftwareFactory.MetaModel", "1.7.0-pre2");
        public static NugetPackageInfo IntentModulesCommon = new NugetPackageInfo("Intent.Modules.Common", "1.5.0");
        public static NugetPackageInfo IntentRoslynWeaverAttributes = new NugetPackageInfo("Intent.RoslynWeaver.Attributes", "1.0.0");
    }
}
