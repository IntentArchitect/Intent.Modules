using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Metadata.ApiGateway.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ApiGatewayRouteModel> GetApiGatewayRouteModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ApiGatewayRouteModel.SpecializationTypeId)
                .Select(x => new ApiGatewayRouteModel(x))
                .ToList();
        }

    }
}