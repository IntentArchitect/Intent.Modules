using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.AppSync.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class GraphQLEndpointFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GraphQLEndpointFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<GraphQLEndpointModel> GraphQLEndpoints => _element.ChildElements
            .GetElementsOfType(GraphQLEndpointModel.SpecializationTypeId)
            .Select(x => new GraphQLEndpointModel(x))
            .ToList();

    }
}