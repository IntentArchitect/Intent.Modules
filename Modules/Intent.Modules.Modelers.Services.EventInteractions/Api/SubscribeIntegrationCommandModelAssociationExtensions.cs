using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class SubscribeIntegrationCommandModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<SubscribeIntegrationCommandTargetEndModel> IntegrationCommandSubscriptions(this IntegrationEventHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SubscribeIntegrationCommandModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SubscribeIntegrationCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<SubscribeIntegrationCommandSourceEndModel> IntegrationCommandHandlers(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SubscribeIntegrationCommandModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SubscribeIntegrationCommandModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}