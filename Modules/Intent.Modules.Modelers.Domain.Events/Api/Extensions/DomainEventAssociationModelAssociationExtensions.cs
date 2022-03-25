using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    public static class DomainEventAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventAssociationTargetEndModel> AssociatedClasses(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DomainEventAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventAssociationSourceEndModel> AssociatedDomainEvents(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DomainEventAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}