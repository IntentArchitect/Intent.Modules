using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Constants;
using Intent.Eventing;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.Owin.Jwt.Decorators
{
    public class JwtAuthOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IHasAssemblyDependencies, ITemplateBeforeExecutionHook
    {
        private readonly IApplicationEventDispatcher _solutionEvents;
        public const string Identifier = "Intent.Owin.Jwt.JwtAuthOwinStartupDecorator";

        public JwtAuthOwinStartupDecorator(IApplicationEventDispatcher solutionEvents)
        {
            _solutionEvents = solutionEvents;
            _solutionEvents.Subscribe(SolutionEvents.ResourceAvailable_IdentityServer, HandleIdentityServerAvailable);
        }


        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "System.IdentityModel.Tokens",
                "System.Configuration",
                "IdentityServer3.AccessTokenValidation",
                "Microsoft.Owin.Security",
                "Microsoft.Owin.Security.Infrastructure",
                "Microsoft.Owin.Security.OAuth",
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "ConfigureAuth(app);"
            };
        }

        public IEnumerable<string> Methods()
        {
            return new[]
            {
                @"
        public void ConfigureAuth(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();

            // Fetches the IssuerName and SigningCertificate on startup from the Authority address
            //app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            //{
            //    Authority = ""https://localhost:44300/identity"",
            //    ValidationMode = ValidationMode.ValidationEndpoint,
            //    RequiredScopes = new[] { ""api"" },
            //});

            // Uses specified IssuerName and SigningCertificate, does not require the identity server to be online during startup
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings[""IdentityServer.Issuer.Authority""],
                IssuerName = ConfigurationManager.AppSettings[""IdentityServer.Issuer.Name""],
                SigningCertificate = SigningCertificate.GetFromX509Store(
                    findType: ConfigurationManager.AppSettings[""IdentityServer.Issuer.SigningCertificate.FindType""],
                    findValue: ConfigurationManager.AppSettings[""IdentityServer.Issuer.SigningCertificate.FindValue""]),
                ValidationMode = ValidationMode.Local,
                RequiredScopes = new[] { ""api"" },
            });
        }"
            };
        }

        public int Priority { get; set; } = 10;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.SystemIdentityModelTokensJwt,
                NugetPackages.IdentityServer3AccessTokenValidation,
            };
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.IdentityModel"),
            };
        }

        public void BeforeTemplateExecution()
        {
            _solutionEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                { "Key", "IdentityServer.Issuer.SigningCertificate.FindType" },
                { "Value", "FindBySubjectName" }
            });
            _solutionEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                { "Key", "IdentityServer.Issuer.SigningCertificate.FindValue" },
                { "Value", "localhost" }
            });
            _solutionEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                { "IdentityServer.Issuer.SigningCertificate.FindValue", "localhost" },
            });
        }

        private void HandleIdentityServerAvailable(ApplicationEvent @event)
        {
            _solutionEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                { "Key", "IdentityServer.Issuer.Name" },
                { "Value", @event.GetValue("AuthorityUrl") }
            });
        }
    }
}