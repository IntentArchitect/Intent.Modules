using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Application.ServiceCallHandlers
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentFrameworkDomain = new NugetPackageInfo("Intent.Framework.Domain", "1.0.0", "net45");
        public static NugetPackageInfo CommonServiceLocator = new NugetPackageInfo("CommonServiceLocator", "2.0.3", "net45");
    }
}