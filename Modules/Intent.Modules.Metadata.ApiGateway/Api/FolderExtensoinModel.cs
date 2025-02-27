using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Metadata.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensoinModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensoinModel(IElement element) : base(element)
        {
        }

        public IList<ApiGatewayRouteModel> ApiGatewayRoutes => _element.ChildElements
            .GetElementsOfType(ApiGatewayRouteModel.SpecializationTypeId)
            .Select(x => new ApiGatewayRouteModel(x))
            .ToList();

    }
}