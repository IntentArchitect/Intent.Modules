using System.Collections.Generic;
using System.Linq;
using Intent.Modules.WebApi.Legacy;
using Intent.Modules.WebApi.Templates.WebApiBadHttpRequestException;
using Intent.SoftwareFactory.MetaModels.Class;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.WebApi.Decorators.Legacy
{
    public class WebApiDistributionJsonValidationDecorator : BaseDistributionDecorator, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.WebApi.Distribution.JsonValidation.Decorator.Legacy";

        public override IEnumerable<string> DeclareUsings(ServiceModel service) => new List<string>
        {
            "using Newtonsoft.Json;",
        };

        public override IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter)
        {
            if (parameter.Type.TypeName == "String")
            {
                return new string[0];
            }
            return new[]
            {
                "[JsonProperty(Required = Required.Always)]"
            };
        }

        public override string BeforeTransaction(ServiceModel service, ServiceOperationModel operation)
        {
            if (operation.UsesRawSignature || operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return @"
                if (payload == null)
                {
                    throw new BadHttpRequestException(""Invalid or empty JSON message received."");
                }
";
        }

        public override int Priority { get; set; } = int.MaxValue;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NewtonsoftJson,
            };
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(WebApiBadHttpRequestExceptionTemplate.Identifier),
            };
        }
    }
}
