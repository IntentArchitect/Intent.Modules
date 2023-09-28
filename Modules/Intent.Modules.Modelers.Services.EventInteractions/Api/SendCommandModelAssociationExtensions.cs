using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class SendCommandModelAssociationExtensions
    {

        [IntentManaged(Mode.Fully)]
        public static IList<SendCommandTargetEndModel> SentCommandDestinations(this SubscribeIntegrationEventTargetEndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendCommandModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SendCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<SendCommandTargetEndModel> SentCommandDestinations(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendCommandModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SendCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<SendCommandSourceEndModel> SentCommandSources(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendCommandModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SendCommandModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}