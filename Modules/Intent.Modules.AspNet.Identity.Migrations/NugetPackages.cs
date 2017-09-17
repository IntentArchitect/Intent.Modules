using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.Identity.Migrations
{
    public class NugetPackages
    {
        public static NugetPackageInfo EntityFramework = new NugetPackageInfo("EntityFramework", "6.1.3", "net45");

        public static NugetPackageInfo MicrosoftAspNetIdentityEntityFramework =new NugetPackageInfo("Microsoft.AspNet.Identity.EntityFramework", "2.2.1", "net452");
    }
}