using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<CommandModel> Commands => _element.ChildElements
            .GetElementsOfType(CommandModel.SpecializationTypeId)
            .Select(x => new CommandModel(x))
            .ToList();

        public IList<QueryModel> Queries => _element.ChildElements
            .GetElementsOfType(QueryModel.SpecializationTypeId)
            .Select(x => new QueryModel(x))
            .ToList();

    }
}