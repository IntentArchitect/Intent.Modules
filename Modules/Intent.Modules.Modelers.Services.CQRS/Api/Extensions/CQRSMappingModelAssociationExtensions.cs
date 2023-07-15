using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    public static class CQRSMappingModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CQRSMappingTargetEndModel> PublishedDomainEvents(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CQRSMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CQRSMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CQRSMappingTargetEndModel> PublishedDomainEvents(this QueryModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CQRSMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CQRSMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CQRSMappingSourceEndModel> DomainEventSources()
        {
            return new List<CQRSMappingSourceEndModel>(); // switch off
        }

    }
}