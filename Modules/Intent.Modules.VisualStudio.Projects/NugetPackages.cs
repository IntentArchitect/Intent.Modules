using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VSProjects
{
    public class NugetPackages
    {
        public static INugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "0.1.4-beta", "net45");
        public static INugetPackageInfo MicrosoftAspNetWebApi = new NugetPackageInfo("Microsoft.AspNet.WebApi", "5.2.3", "net45");
        public static INugetPackageInfo MicrosoftAspNetWebApiClient = new NugetPackageInfo("Microsoft.AspNet.WebApi.Client", "5.2.3", "net45");
        public static INugetPackageInfo MicrosoftAspNetWebApiCore = new NugetPackageInfo("Microsoft.AspNet.WebApi.Core", "5.2.3", "net45")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Web.Http", "5.2.3.0", "31bf3856ad364e35"));
        public static INugetPackageInfo MicrosoftAspNetWebApiWebHost = new NugetPackageInfo("Microsoft.AspNet.WebApi.WebHost", "5.2.3", "net45");
        public static INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "9.0.1", "net452")
    .WithAssemblyRedirect(new AssemblyRedirectInfo("Newtonsoft.Json", "9.0.0.0", "30ad4fe6b2a6aeed"));

    }
}
