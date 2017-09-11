using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Application.ServiceCallHandlers
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "0.1.4-beta", "net45");
        public static NugetPackageInfo IntentFrameworkDomain = new NugetPackageInfo("Intent.Framework.Domain","0.1.7-beta", "net45");
        public static NugetPackageInfo CommonServiceLocator = new NugetPackageInfo("CommonServiceLocator", "1.3", "net45");
        public static NugetPackageInfo Unity = new NugetPackageInfo("Unity", "4.0.1", "net45");
    }
}