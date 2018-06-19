using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.WebApi
{
    public class NugetPackages
    {
        public static NugetPackageInfo MicrosoftWebInfrastructure = new NugetPackageInfo("Microsoft.Web.Infrastructure", "1.0.0", "net45");
        
        public static NugetPackageInfo EntityFramework = new NugetPackageInfo("EntityFramework", "6.1.3", "net45");

        public static NugetPackageInfo IdentityServer3 = new NugetPackageInfo("IdentityServer3", "2.5.0", "net452");

        public static NugetPackageInfo IdentityServer3AspNetIdentity = new NugetPackageInfo("IdentityServer3.AspNetIdentity", "2.0.0", "net452");
         
        public static NugetPackageInfo IdentityServer3AccessTokenValidation = new NugetPackageInfo("IdentityServer3.AccessTokenValidation", "2.10.0", "net452");

        public static NugetPackageInfo SystemIdentityModelTokensJwt = new NugetPackageInfo("System.IdentityModel.Tokens.Jwt", "4.0.2.206221351", "net452")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.IdentityModel.Tokens.Jwt", "4.0.20622.1351", "31bf3856ad364e35"));

        public static NugetPackageInfo IntentEsbClient = new NugetPackageInfo("Intent.Esb.Client", "0.1.14-beta","net45");

        public static NugetPackageInfo IntentEsbServer = new NugetPackageInfo("Intent.Esb.Server", "0.1.14-beta","net45");

        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0", "net45");

        public static NugetPackageInfo IntentFrameworkDomain = new NugetPackageInfo("Intent.Framework.Domain", "1.0.0", "net45");

        public static NugetPackageInfo IntentFrameworkEntityFramework = new NugetPackageInfo("Intent.Framework.EntityFramework", "1.0.0", "net45");
        
        public static NugetPackageInfo IntentFrameworkUnity = new NugetPackageInfo("Intent.Framework.Unity","0.1.2-beta", "net45");

        public static NugetPackageInfo IntentFrameworkWcf = new NugetPackageInfo("Intent.Framework.WCF", "0.1.4-beta","net45");

        public static NugetPackageInfo IntentFrameworkAutoMapper = new NugetPackageInfo("Intent.Framework.AutoMapper","0.1.1-beta", "net45");

        public static NugetPackageInfo MicrosoftAspNetIdentityCore =new NugetPackageInfo("Microsoft.AspNet.Identity.Core", "2.2.1", "net452");

        public static NugetPackageInfo MicrosoftAspNetIdentityEntityFramework =new NugetPackageInfo("Microsoft.AspNet.Identity.EntityFramework", "2.2.1", "net452");

        public static NugetPackageInfo MicrosoftAspNetWebApi = new NugetPackageInfo("Microsoft.AspNet.WebApi", "5.2.6","net45");

        public static NugetPackageInfo MicrosoftAspNetWebApiClient =new NugetPackageInfo("Microsoft.AspNet.WebApi.Client", "5.2.6", "net45");

        public static NugetPackageInfo MicrosoftAspNetWebApiCore = new NugetPackageInfo("Microsoft.AspNet.WebApi.Core","5.2.6", "net45")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Web.Http", "5.2.6.0", "31bf3856ad364e35"));

        public static NugetPackageInfo MicrosoftAspNetWebApiWebHost =new NugetPackageInfo("Microsoft.AspNet.WebApi.WebHost", "5.2.6", "net45");

        public static NugetPackageInfo MicrosoftOwin = new NugetPackageInfo("Microsoft.Owin", "4.0.0", "net45")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Microsoft.Owin", "4.0.0.0", "31bf3856ad364e35"));

        public static NugetPackageInfo MicrosoftOwinFileSystems = new NugetPackageInfo("Microsoft.Owin.FileSystems","3.0.1", "net45");

        public static NugetPackageInfo MicrosoftOwinCors = new NugetPackageInfo("Microsoft.Owin.Cors", "3.0.1", "net45");

        public static NugetPackageInfo MicrosoftOwinHostSystemWeb = new NugetPackageInfo("Microsoft.Owin.Host.SystemWeb", "4.0.0", "net45");

        public static NugetPackageInfo MicrosoftOwinStaticFiles = new NugetPackageInfo("Microsoft.Owin.StaticFiles","3.0.1", "net45");

        public static NugetPackageInfo MicrosoftAspNetWebApiOwin = new NugetPackageInfo("Microsoft.AspNet.WebApi.Owin","5.2.6", "net452");

        public static NugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "9.0.1", "net452")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Newtonsoft.Json", "9.0.0.0", "30ad4fe6b2a6aeed"));

        public static NugetPackageInfo NLog = new NugetPackageInfo("NLog", "4.3.4", "net452");

        public static NugetPackageInfo UnityWebApi = new NugetPackageInfo("Unity.WebAPI", "5.3.0", "net452")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo UnityAspNetWebApi = new NugetPackageInfo("Unity.AspNet.WebApi", "4.0.1", "net452")
            .BlockAddingOfAllFiles();

        public static NugetPackageInfo NWebsecOwin = new NugetPackageInfo("NWebsec.Owin", "2.2.0", "net452");
        public static NugetPackageInfo Topshelf = new NugetPackageInfo("Topshelf", "3.3.1", "net452");
        public static NugetPackageInfo TopshelfNLog = new NugetPackageInfo("Topshelf.NLog", "3.3.1", "net452");
    }
}