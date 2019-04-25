using Intent.MetaModel.Hosting;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Constants;
using Intent.Modules.IdentityServer.Templates.AspNetIdentityModel;
using Intent.Modules.IdentityServer.Templates.AspNetIdentityUserService;
using Intent.Modules.IdentityServer.Templates.Clients;
using Intent.Modules.IdentityServer.Templates.Scopes;
using Intent.Modules.IdentityServer.Templates.SigningCertificate;
using Intent.Eventing;
using Intent.Templates
using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.IdentityServer.Decorators
{
    public class IdentityServerOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IHasTemplateDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.IdentityServer.OwinStartupDecorator";

        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _applicationEvents;
        private readonly ISolutionEventDispatcher _solutionEvents;

        public IdentityServerOwinStartupDecorator(HostingConfigModel hostingConfig, IApplicationEventDispatcher applicationEvents, ISolutionEventDispatcher solutionEvents)
        {
            _hostingConfig = hostingConfig ?? new HostingConfigModel() { UseSsl = true, SslPort = "44399" };
            _applicationEvents = applicationEvents;
            _solutionEvents = solutionEvents;
            Priority = 10;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "System.Configuration",
                "IdentityServer3.Core.Configuration",
                "IdentityServer3.Core.Services",
                "IdentityServer3.Core.Services.Default",
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "app.Map(\"/identity\", ConfigureIdentityServer);",
            };
        }

        public IEnumerable<string> Methods()
        {
            return new[]
           {
                $@"
        private static void ConfigureIdentityServer(IAppBuilder app)
        {{
            var factory = new IdentityServerServiceFactory()
                .ConfigureUserService(connectionString: ""{AspNetIdentityModelTemplate.DB_CONTEXT_NAME}"")
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            factory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService {{AllowAll = true}});

            app.UseIdentityServer(new IdentityServerOptions
            {{
                SiteName = ""IdentityServer"",
                SigningCertificate = SigningCertificate.GetFromX509Store(
                    findType: ConfigurationManager.AppSettings[""IdentityServer.Issuer.SigningCertificate.FindType""],
                    findValue: ConfigurationManager.AppSettings[""IdentityServer.Issuer.SigningCertificate.FindValue""]),
                Factory = factory
            }});
        }}",
            };
        }

        public int Priority { get; set; }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IdentityServer3,
            };
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(SigningCertificateTemplate.Identifier),
                TemplateDependency.OnTemplate(IdentityServerScopesTemplate.Identifier),
                TemplateDependency.OnTemplate(IdentityServerClientsTemplate.Identifier),
                TemplateDependency.OnTemplate(AspNetIdentityUserServiceTemplate.Identifier)
            };
        }

        public void PreProcess()
        {
            _solutionEvents.Publish(SolutionEvents.ResourceAvailable_IdentityServer, new Dictionary<string, string>()
            {
                { "AuthorityUrl", $"{_hostingConfig.GetBaseUrl()}/identity" },
                { "BaseUrl", _hostingConfig.GetBaseUrl() }
            });
            _applicationEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                {"Key", "IdentityServer.Issuer.Name"},
                {"Value", $"{_hostingConfig.GetBaseUrl()}/identity" }
            });
            _applicationEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                {"Key", "IdentityServer.Issuer.SigningCertificate.FindType"},
                {"Value", "FindBySubjectName" }
            });
            _applicationEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                {"Key", "IdentityServer.Issuer.SigningCertificate.FindValue"},
                {"Value", "localhost" }
            });
        }
    }
}