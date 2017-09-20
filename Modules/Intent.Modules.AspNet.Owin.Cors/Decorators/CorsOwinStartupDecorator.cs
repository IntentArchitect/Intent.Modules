using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.Owin.Cors.Decorators
{
    public class CorsOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Owin.Cors.OwinStartupDecorator";

        public CorsOwinStartupDecorator()
        {
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using Microsoft.Owin.Cors;"
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "ConfigureCors(app);"
            };
        }

        public IEnumerable<string> Methods()
        {
            return new[]
            {
                @"
        private static void ConfigureCors(IAppBuilder app)
        {
            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true
            };

            // Try and load allowed origins from web.config
            // If none are specified we'll allow all origins

            var origins = ConfigurationManager.AppSettings[""CorsAllowedOrigins""];

            if (origins != null)
            {
                foreach (var origin in origins.Split(';'))
                {
                    corsPolicy.Origins.Add(origin);
                }
            }
            else
            {
                corsPolicy.AllowAnyOrigin = true;
            }

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            };

            app.UseCors(corsOptions);
        }"
            };
        }

        public int Priority()
        {
            return 0;
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftOwinCors,
            };
        }

        public void PreProcess()
        {
            // TODO: need metadata for who to allow.
            //_eventDispatcher.Publish(new WebConfigAppSettingRequiredEvent(_applicationName, "IdentityServer.Issuer.Name", $"{_application.HostingConfig.GetBaseUrl()}/identity"))
        }

        int IPriorityDecorator.Priority { get; set; } = 20;
    }
}