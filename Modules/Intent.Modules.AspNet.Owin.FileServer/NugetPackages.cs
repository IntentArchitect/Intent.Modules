using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Owin.FileServer
{
    public class NugetPackages
    {
        public static NugetPackageInfo MicrosoftOwinFileSystems = new NugetPackageInfo("Microsoft.Owin.FileSystems","3.0.1", "net45");
        public static NugetPackageInfo MicrosoftOwinStaticFiles = new NugetPackageInfo("Microsoft.Owin.StaticFiles", "3.0.1", "net45");
    }
}