using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    public static class DomainEventHandlerAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventHandlerAssociationTargetEndModel> PublishedDomainEvents(this ServiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventHandlerAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DomainEventHandlerAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventHandlerAssociationSourceEndModel> DomainEventSources(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventHandlerAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DomainEventHandlerAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}