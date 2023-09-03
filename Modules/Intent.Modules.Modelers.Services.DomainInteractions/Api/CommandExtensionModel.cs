using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CommandExtensionModel : CommandModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommandExtensionModel(IElement element) : base(element)
        {
        }

        public IList<ProcessingActionModel> ProcessingActions => _element.ChildElements
            .GetElementsOfType(ProcessingActionModel.SpecializationTypeId)
            .Select(x => new ProcessingActionModel(x))
            .ToList();

    }
}