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
    public class AWSAPIGatewayModuleBuilderFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSAPIGatewayModuleBuilderFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<APIMethodTypeModel> APIMethodTypes => _element.ChildElements
            .GetElementsOfType(APIMethodTypeModel.SpecializationTypeId)
            .Select(x => new APIMethodTypeModel(x))
            .ToList();

    }
}