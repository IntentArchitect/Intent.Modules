using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ServerlessAWSFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerlessAWSFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<AWSAPIGatewayEndpointModel> APIGatewayEndpoints => _element.ChildElements
            .GetElementsOfType(AWSAPIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new AWSAPIGatewayEndpointModel(x))
            .ToList();

        public IList<DTOModel> DTOs => _element.ChildElements
            .GetElementsOfType(DTOModel.SpecializationTypeId)
            .Select(x => new DTOModel(x))
            .ToList();

        public IList<AWSLambdaFunctionModel> LambdaFunctions => _element.ChildElements
            .GetElementsOfType(AWSLambdaFunctionModel.SpecializationTypeId)
            .Select(x => new AWSLambdaFunctionModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => _element.ChildElements
            .GetElementsOfType(DiagramModel.SpecializationTypeId)
            .Select(x => new DiagramModel(x))
            .ToList();

        public IList<AWSSimpleQueueServiceQueueModel> SimpleQueueServiceQueues => _element.ChildElements
            .GetElementsOfType(AWSSimpleQueueServiceQueueModel.SpecializationTypeId)
            .Select(x => new AWSSimpleQueueServiceQueueModel(x))
            .ToList();

    }
}