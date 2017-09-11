using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Mapping.EntityToDto
{
    public class NugetPackages
    {
        public static NugetPackageInfo AutoMapper = new NugetPackageInfo("AutoMapper", "4.2.1", "net45");
        public static NugetPackageInfo IntentFrameworkAutoMapper = new NugetPackageInfo("Intent.Framework.AutoMapper", "0.1.1-beta", "net45");
    }
}