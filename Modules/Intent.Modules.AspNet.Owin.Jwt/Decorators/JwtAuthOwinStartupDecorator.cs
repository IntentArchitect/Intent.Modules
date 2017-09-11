using System.Collections.Generic;
using Intent.Packages.Constants;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Owin.Jwt.Decorators
{
    public class JwtAuthOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IHasAssemblyDependencies, IRequiresPreProcessing
    {
        private readonly ISolutionEventDispatcher _solutionEvents;
        public const string Identifier = "Intent.Owin.Jwt.JwtAuthOwinStartupDecorator";

        public JwtAuthOwinStartupDecorator(ISolutionEventDispatcher solutionEvents)
        {
            _solutionEvents = solutionEvents;
            _solutionEvents.Subscribe(SolutionEvents.ResourceAvailable_IdentityServer, HandleIdentityServerAvailable);
        }


        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using System.IdentityModel.Tokens;",
                "using System.Configuration;",
                "using IdentityServer3.AccessTokenValidation;",
                "using Microsoft.Owin.Security;",
                "using Microsoft.Owin.Security.Infrastructure;",
                "using Microsoft.Owin.Security.OAuth;",
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

        public void PreProcess()
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

        private void HandleIdentityServerAvailable(SolutionEvent @event)
        {
            _solutionEvents.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                { "Key", "IdentityServer.Issuer.Name" },
                { "Value", @event.GetValue("AuthorityUrl") }
            });
        }
    }
}