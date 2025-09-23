using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    public static class DomainEventGeneralizationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventGeneralizationTargetEndModel> Generalizations(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventGeneralizationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DomainEventGeneralizationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventGeneralizationSourceEndModel> Specializations(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DomainEventGeneralizationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DomainEventGeneralizationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DomainEventGeneralizationEndModel> DomainEventGeneralizationEnds(this DomainEventModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsDomainEventGeneralizationEndModel())
                .Select(DomainEventGeneralizationEndModel.Create)
                .ToList();
        }

    }
}