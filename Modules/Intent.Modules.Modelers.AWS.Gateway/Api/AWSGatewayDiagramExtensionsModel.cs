using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Gateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSGatewayDiagramExtensionsModel : DiagramModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSGatewayDiagramExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<APIGatewayEndpointModel> APIEndpoints => _element.ChildElements
            .GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new APIGatewayEndpointModel(x))
            .ToList();

    }
}