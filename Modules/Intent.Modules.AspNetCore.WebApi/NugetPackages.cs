using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNetCore.WebApi
{
    public class NugetPackages
    {
        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0");
        public static NugetPackageInfo AspNetCoreAll = new NugetPackageInfo("Microsoft.AspNetCore.All", "2.0.8");
    }
}