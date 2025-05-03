using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    public static class ServiceInvocationModelAssociationExtensions
    {
        public static IList<ServiceInvocationTargetEndModel> ServiceInvocationActions(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ServiceInvocationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ServiceInvocationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
    }
}