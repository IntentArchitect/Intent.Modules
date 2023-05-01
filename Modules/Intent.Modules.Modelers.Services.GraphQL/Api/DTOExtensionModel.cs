using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.GraphQL.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DTOExtensionModel : DTOModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DTOExtensionModel(IElement element) : base(element)
        {
        }

        public IList<GraphQLSchemaFieldModel> Resolvers => _element.ChildElements
            .GetElementsOfType(GraphQLSchemaFieldModel.SpecializationTypeId)
            .Select(x => new GraphQLSchemaFieldModel(x))
            .ToList();

    }
}