using System.Collections.Generic;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.AspNet.Swashbuckle.Decorators
{
    public class SwashbuckleWebApiConfigTemplateDecorator : WebApiConfigTemplateDecoratorBase, IHasNugetDependencies, IDeclareUsings
    {
        private readonly IApplication _application;

        public SwashbuckleWebApiConfigTemplateDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Id = "Intent.AspNet.Swashbuckle.WebApiConfigDecorator";

        public override IEnumerable<string> Configure()
        {
            return new[]
            {
                "ConfigureSwashbuckle(config);",
            };
        }

        public override string Methods()
        {
            return $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private static void ConfigureSwashbuckle(HttpConfiguration config)
        {{
            config.EnableSwagger(c => c.SingleApiVersion(""v1"", ""{_application.Name} API""))
                .EnableSwaggerUi();
        }}";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Swashbuckle.Application",
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] 
            {
                NugetPackages.Swashbuckle,
            };
        }
    }
}
