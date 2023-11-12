using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class CallServiceOperationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CallServiceOperationTargetEndModel> CalledServiceOperations(this SubscribeIntegrationEventTargetEndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallServiceOperationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CallServiceOperationSourceEndModel> CallServiceOperationSources(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CallServiceOperationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}