using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore
{
    public static class NugetPackages
    {
        public static NugetPackageInfo EntityFrameworkCore = new NugetPackageInfo("Microsoft.EntityFrameworkCore", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreDesign = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Design", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreTools = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Tools", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreSqlServer = new NugetPackageInfo("Microsoft.EntityFrameworkCore.SqlServer", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreInMemory = new NugetPackageInfo("Microsoft.EntityFrameworkCore.InMemory", "2.1.1");
        public static NugetPackageInfo EntityFrameworkCoreProxies = new NugetPackageInfo("Microsoft.EntityFrameworkCore.Proxies", "2.1.1");
    }
}
