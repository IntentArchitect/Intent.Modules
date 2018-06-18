using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.RichDomain.EntityFramework
{
    public class NugetPackages
    {
        public static NugetPackageInfo EntityFramework = new NugetPackageInfo("EntityFramework", "6.1.3", "net45");

        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0", "net45");

        public static NugetPackageInfo IntentFrameworkDomain = new NugetPackageInfo("Intent.Framework.Core", "1.0.0", "net45");

        public static NugetPackageInfo IntentFrameworkEntityFramework = new NugetPackageInfo("Intent.Framework.EntityFramework", "0.1.8-beta", "net45");


    }
}