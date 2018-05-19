using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Application.Contracts.Mappings
{
    public static class NugetPackages
    {
        public static INugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0-pre3", "net45");
        public static INugetPackageInfo AutoMapper = new NugetPackageInfo("AutoMapper", "4.2.1", "net45");

    }
}