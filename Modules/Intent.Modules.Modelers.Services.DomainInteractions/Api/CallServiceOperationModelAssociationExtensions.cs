using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class CallServiceOperationModelAssociationExtensions
    {
        public static IList<CallServiceOperationTargetEndModel> CallServiceOperationActions(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallServiceOperationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
    }
}