using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.Owin.Cors.Decorators
{
    public class CorsOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Owin.Cors.OwinStartupDecorator";

        public CorsOwinStartupDecorator()
        {
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.Owin.Cors"
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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftOwinCors,
            };
        }

        public void BeforeTemplateExecution()
        {
            // TODO: need metadata for who to allow.
            //_eventDispatcher.Publish(new WebConfigAppSettingRequiredEvent(_applicationName, "IdentityServer.Issuer.Name", $"{_application.HostingConfig.GetBaseUrl()}/identity"))
        }

        public int Priority { get; } = 20;
    }
}