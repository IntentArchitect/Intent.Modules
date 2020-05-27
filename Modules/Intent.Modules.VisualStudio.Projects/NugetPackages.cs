using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.VisualStudio.Projects
{
    public class NugetPackages
    {
        public static INugetPackageInfo MicrosoftAspNetWebApi = new NugetPackageInfo("Microsoft.AspNet.WebApi", "5.2.6");
        public static INugetPackageInfo MicrosoftAspNetWebApiClient = new NugetPackageInfo("Microsoft.AspNet.WebApi.Client", "5.2.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Net.Http.Formatting", "5.2.6.0", "31bf3856ad364e35"));
        public static INugetPackageInfo MicrosoftAspNetWebApiCore = new NugetPackageInfo("Microsoft.AspNet.WebApi.Core", "5.2.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Web.Http", "5.2.6.0", "31bf3856ad364e35"));
        public static INugetPackageInfo MicrosoftAspNetWebApiWebHost = new NugetPackageInfo("Microsoft.AspNet.WebApi.WebHost", "5.2.6");
        public static INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "9.0.1")
    .WithAssemblyRedirect(new AssemblyRedirectInfo("Newtonsoft.Json", "9.0.0.0", "30ad4fe6b2a6aeed"));

    }
}
