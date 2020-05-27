using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HttpServiceProxy
{
    public class NugetPackages
    {
        public static NugetPackageInfo MicrosoftAspNetWebApiClient = new NugetPackageInfo("Microsoft.AspNet.WebApi.Client", "5.2.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Net.Http.Formatting", "5.2.6.0", "31bf3856ad364e35"));
            
    }
}