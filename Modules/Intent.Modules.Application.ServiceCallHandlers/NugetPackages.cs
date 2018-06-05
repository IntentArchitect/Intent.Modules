using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Application.ServiceCallHandlers
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0-pre3", "net45");
        public static NugetPackageInfo IntentFrameworkDomain = new NugetPackageInfo("Intent.Framework.Domain", "1.0.0-pre2", "net45");
        public static NugetPackageInfo CommonServiceLocator = new NugetPackageInfo("CommonServiceLocator", "1.3", "net45");
        public static NugetPackageInfo Unity = new NugetPackageInfo("Unity", "4.0.1", "net45");
    }
}