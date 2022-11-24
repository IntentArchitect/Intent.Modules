using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<APIGatewayEndpointModel> GetAPIGatewayEndpointModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
                .Select(x => new APIGatewayEndpointModel(x))
                .ToList();
        }

        public static IList<DiagramModel> GetDiagramModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DiagramModel.SpecializationTypeId)
                .Select(x => new DiagramModel(x))
                .ToList();
        }

        public static IList<DTOModel> GetDTOModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DTOModel.SpecializationTypeId)
                .Select(x => new DTOModel(x))
                .ToList();
        }

        public static IList<LambdaFunctionModel> GetLambdaFunctionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
                .Select(x => new LambdaFunctionModel(x))
                .ToList();
        }

        public static IList<SQSQueueModel> GetSQSQueueModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SQSQueueModel.SpecializationTypeId)
                .Select(x => new SQSQueueModel(x))
                .ToList();
        }

    }
}