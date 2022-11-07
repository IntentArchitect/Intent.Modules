using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class MessagePublishAssocationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<MessagePublishAssocationTargetEndModel> PublishedMessages(this ApplicationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == MessagePublishAssocationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => MessagePublishAssocationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<MessagePublishAssocationSourceEndModel> PublishingApplications(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == MessagePublishAssocationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => MessagePublishAssocationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}