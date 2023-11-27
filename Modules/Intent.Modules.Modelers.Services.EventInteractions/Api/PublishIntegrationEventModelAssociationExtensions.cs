using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class PublishIntegrationEventModelAssociationExtensions
    {
        public static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsTargetEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        public static IList<PublishIntegrationEventSourceEndModel> IntegrationEventsSources(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == PublishIntegrationEventModel.SpecializationType && x.IsSourceEnd())
                .Select(x => PublishIntegrationEventModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}