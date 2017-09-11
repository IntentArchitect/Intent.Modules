using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Package.Logging.NLog
{
    public class NugetPackages
    {
        public static NugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "9.0.1", "net452")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Newtonsoft.Json", "9.0.0.0", "30ad4fe6b2a6aeed"));

        public static NugetPackageInfo NLog = new NugetPackageInfo("NLog", "4.3.4", "net452");
    }
}