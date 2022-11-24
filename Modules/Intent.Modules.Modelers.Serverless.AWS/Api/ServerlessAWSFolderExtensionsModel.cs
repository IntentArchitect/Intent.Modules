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

        public IList<APIGatewayEndpointModel> APIEndpoints => _element.ChildElements
            .GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new APIGatewayEndpointModel(x))
            .ToList();

        public IList<DTOModel> DTOs => _element.ChildElements
            .GetElementsOfType(DTOModel.SpecializationTypeId)
            .Select(x => new DTOModel(x))
            .ToList();

        public IList<LambdaFunctionModel> LambdaFunctions => _element.ChildElements
            .GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
            .Select(x => new LambdaFunctionModel(x))
            .ToList();

        public IList<SQSQueueModel> SQSQueues => _element.ChildElements
            .GetElementsOfType(SQSQueueModel.SpecializationTypeId)
            .Select(x => new SQSQueueModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => _element.ChildElements
            .GetElementsOfType(DiagramModel.SpecializationTypeId)
            .Select(x => new DiagramModel(x))
            .ToList();

    }
}