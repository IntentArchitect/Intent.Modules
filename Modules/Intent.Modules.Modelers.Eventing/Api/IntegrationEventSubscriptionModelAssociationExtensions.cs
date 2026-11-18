using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class IntegrationEventSubscriptionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationEventSubscriptionTargetEndModel> PublishedIntegrationEvent(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == IntegrationEventSubscriptionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => IntegrationEventSubscriptionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationEventSubscriptionSourceEndModel> SubscribingIntegrationEvents(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == IntegrationEventSubscriptionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => IntegrationEventSubscriptionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationEventSubscriptionEndModel> IntegrationEventSubscriptionEnds(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsIntegrationEventSubscriptionEndModel())
                .Select(IntegrationEventSubscriptionEndModel.Create)
                .ToList();
        }

    }
}