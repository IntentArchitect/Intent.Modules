using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    public static class CQRSSubscriptionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CQRSSubscriptionTargetEndModel> PublishedDomainEvents(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CQRSSubscriptionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CQRSSubscriptionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        //[IntentManaged(Mode.Fully)]
        //public static IList<CQRSSubscriptionSourceEndModel> DomainEventSources(this MessageModel model)
        //{
        //    return model.InternalElement.AssociatedElements
        //        .Where(x => x.Association.SpecializationType == CQRSSubscriptionModel.SpecializationType && x.IsSourceEnd())
        //        .Select(x => CQRSSubscriptionModel.CreateFromEnd(x).SourceEnd)
        //        .ToList();
        //}

    }
}