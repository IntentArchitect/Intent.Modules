using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Merge)]
    public class DiagramExtensionModel : DiagramModel
    {
        [IntentManaged(Mode.Ignore)]
        public DiagramExtensionModel(IElement element) : base(element)
        {
        }

        public IList<DomainEventModel> DomainEvents => _element.ChildElements
            .GetElementsOfType(DomainEventModel.SpecializationTypeId)
            .Select(x => new DomainEventModel(x))
            .ToList();

    }
}