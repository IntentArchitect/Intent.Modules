using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    public static class CallExternalEndpointModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CallExternalEndpointTargetEndModel> CallServiceOperationAction(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallExternalEndpointModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallExternalEndpointModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
    }
}