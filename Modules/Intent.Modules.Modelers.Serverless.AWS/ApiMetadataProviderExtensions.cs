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
        public static IList<AWSAPIGatewayEndpointModel> GetAWSAPIGatewayEndpointModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AWSAPIGatewayEndpointModel.SpecializationTypeId)
                .Select(x => new AWSAPIGatewayEndpointModel(x))
                .ToList();
        }

        public static IList<AWSLambdaFunctionModel> GetAWSLambdaFunctionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AWSLambdaFunctionModel.SpecializationTypeId)
                .Select(x => new AWSLambdaFunctionModel(x))
                .ToList();
        }

        public static IList<AWSSimpleQueueServiceQueueModel> GetAWSSimpleQueueServiceQueueModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AWSSimpleQueueServiceQueueModel.SpecializationTypeId)
                .Select(x => new AWSSimpleQueueServiceQueueModel(x))
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

    }
}