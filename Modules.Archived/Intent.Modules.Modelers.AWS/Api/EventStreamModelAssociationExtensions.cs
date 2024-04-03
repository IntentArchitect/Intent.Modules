using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    public static class EventStreamModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<EventStreamTargetEndModel> AssociationTargetEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == EventStreamModel.SpecializationType && x.IsTargetEnd())
                .Select(x => EventStreamModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<EventStreamSourceEndModel> AssociationSourceEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == EventStreamModel.SpecializationType && x.IsSourceEnd())
                .Select(x => EventStreamModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<EventStreamEndModel> EventStreamEnds(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsEventStreamEndModel())
                .Select(EventStreamEndModel.Create)
                .ToList();
        }

    }
}