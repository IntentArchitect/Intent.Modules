using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Gateway.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<APIGatewayEndpointModel> GetAPIGatewayEndpointModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
                .Select(x => new APIGatewayEndpointModel(x))
                .ToList();
        }

    }
}