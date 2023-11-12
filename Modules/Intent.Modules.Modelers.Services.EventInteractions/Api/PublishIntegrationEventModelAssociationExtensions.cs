using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class PublishIntegrationEventModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this DomainEventHandlerAssociationTargetEndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this SubscribeIntegrationEventTargetEndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<PublishIntegrationEventSourceEndModel> IntegrationEventsSources(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsSourceEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}