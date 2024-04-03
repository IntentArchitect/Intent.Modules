using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.ApiGateway.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<APIGatewayModel> GetAPIGatewayModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(APIGatewayModel.SpecializationTypeId)
                .Select(x => new APIGatewayModel(x))
                .ToList();
        }

        public static IList<APIMethodTypeModel> GetAPIMethodTypeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(APIMethodTypeModel.SpecializationTypeId)
                .Select(x => new APIMethodTypeModel(x))
                .ToList();
        }

    }
}