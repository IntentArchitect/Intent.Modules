using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSAPIGatewayFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSAPIGatewayFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<APIGatewayModel> APIGateways => _element.ChildElements
            .GetElementsOfType(APIGatewayModel.SpecializationTypeId)
            .Select(x => new APIGatewayModel(x))
            .ToList();

    }
}