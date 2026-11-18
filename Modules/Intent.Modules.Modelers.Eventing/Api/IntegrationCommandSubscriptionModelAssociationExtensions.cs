using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class IntegrationCommandSubscriptionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationCommandSubscriptionTargetEndModel> PublishedIntegrationCommand(this IntegrationCommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == IntegrationCommandSubscriptionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => IntegrationCommandSubscriptionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationCommandSubscriptionSourceEndModel> SubscribingIntegrationCommands(this IntegrationCommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == IntegrationCommandSubscriptionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => IntegrationCommandSubscriptionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<IntegrationCommandSubscriptionEndModel> IntegrationCommandSubscriptionEnds(this IntegrationCommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsIntegrationCommandSubscriptionEndModel())
                .Select(IntegrationCommandSubscriptionEndModel.Create)
                .ToList();
        }

    }
}