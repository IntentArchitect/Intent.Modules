using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<MessageModel> Messages => _element.ChildElements
            .GetElementsOfType(MessageModel.SpecializationTypeId)
            .Select(x => new MessageModel(x))
            .ToList();

        public IList<IntegrationCommandModel> IntegrationCommands => _element.ChildElements
            .GetElementsOfType(IntegrationCommandModel.SpecializationTypeId)
            .Select(x => new IntegrationCommandModel(x))
            .ToList();

        public IList<EventingDTOModel> EventingDTOs => _element.ChildElements
            .GetElementsOfType(EventingDTOModel.SpecializationTypeId)
            .Select(x => new EventingDTOModel(x))
            .ToList();

    }
}