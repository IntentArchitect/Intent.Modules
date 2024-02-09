using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class CallServiceOperationActionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CallServiceOperationActionTargetEndModel> CallServiceOperationActionTargets(this ComponentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallServiceOperationActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CallServiceOperationActionTargetEndModel> CallServiceOperationActionTargets(this ComponentCommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallServiceOperationActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CallServiceOperationActionSourceEndModel> CallServiceOperationActionSources(this ServiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CallServiceOperationActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}