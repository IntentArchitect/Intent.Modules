using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    public static class DomainEventOriginAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventOriginAssociationTargetEndModel> PublishedDomainEvents(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventOriginAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DomainEventOriginAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventOriginAssociationTargetEndModel> PublishedDomainEvents(this ClassConstructorModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventOriginAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DomainEventOriginAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventOriginAssociationSourceEndModel> DomainEventSources(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventOriginAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DomainEventOriginAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}