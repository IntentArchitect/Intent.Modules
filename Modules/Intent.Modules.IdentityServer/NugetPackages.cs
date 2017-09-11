using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.IdentityServer
{
    public class NugetPackages
    {
        public static NugetPackageInfo IdentityServer3 = new NugetPackageInfo("IdentityServer3", "2.5.0", "net452");

        public static NugetPackageInfo IdentityServer3AspNetIdentity = new NugetPackageInfo("IdentityServer3.AspNetIdentity", "2.0.0", "net452");
         
        public static NugetPackageInfo IdentityServer3AccessTokenValidation = new NugetPackageInfo("IdentityServer3.AccessTokenValidation", "2.10.0", "net452");

        public static NugetPackageInfo MicrosoftAspNetIdentityCore = new NugetPackageInfo("Microsoft.AspNet.Identity.Core", "2.2.1", "net452");

        public static NugetPackageInfo MicrosoftAspNetIdentityEntityFramework = new NugetPackageInfo("Microsoft.AspNet.Identity.EntityFramework", "2.2.1", "net452");
    }
}