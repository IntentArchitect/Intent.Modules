using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore
{
    public static class NugetPackages
    {
        public static NugetPackageInfo EntityFrameworkCore = new NugetPackageInfo("Microsoft.EntityFrameworkCore.SqlServer", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreProxies = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Proxies", "2.1.1");
    }
}
