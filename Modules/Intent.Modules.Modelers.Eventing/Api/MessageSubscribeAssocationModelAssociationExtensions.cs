using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class MessageSubscribeAssocationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<MessageSubscribeAssocationTargetEndModel> SubscribedMessages(this ApplicationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == MessageSubscribeAssocationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => MessageSubscribeAssocationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<MessageSubscribeAssocationSourceEndModel> ConsumingApplications(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == MessageSubscribeAssocationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => MessageSubscribeAssocationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}