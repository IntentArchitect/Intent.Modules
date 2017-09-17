using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.Owin.Jwt
{
    public static class NugetPackages
    {
        public static NugetPackageInfo SystemIdentityModelTokensJwt = new NugetPackageInfo("System.IdentityModel.Tokens.Jwt", "4.0.2.206221351", "net452")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.IdentityModel.Tokens.Jwt", "4.0.20622.1351", "31bf3856ad364e35"));
        public static NugetPackageInfo IdentityServer3AccessTokenValidation = new NugetPackageInfo("IdentityServer3.AccessTokenValidation", "2.10.0", "net452");
    }
}