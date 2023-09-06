using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class SubscribeIntegrationEventModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<SubscribeIntegrationEventSourceEndModel> IntegrationEventHandlers(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SubscribeIntegrationEventModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SubscribeIntegrationEventModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<SubscribeIntegrationEventTargetEndModel> IntegrationEventsSubscriptions(this IntegrationEventHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SubscribeIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SubscribeIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

    }
}