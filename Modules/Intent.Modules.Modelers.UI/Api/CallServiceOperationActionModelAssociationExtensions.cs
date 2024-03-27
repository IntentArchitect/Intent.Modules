using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class CallServiceOperationActionModelAssociationExtensions
    {
        public static IList<CallServiceOperationActionTargetEndModel> CallServiceOperationActionTargets(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallServiceOperationActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallServiceOperationActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        public static IElementToElementMapping GetMapInvocationMapping(this CallServiceOperationActionTargetEndModel model)
        {
            return model.Mappings.SingleOrDefault(x => x.TypeId == "e4a4111b-cf13-4efe-8a5d-fea9457ac8ad");
        }

        public static IElementToElementMapping GetMapResponseMapping(this CallServiceOperationActionTargetEndModel model)
        {
            return model.Mappings.SingleOrDefault(x => x.TypeId == "6f998e3e-6e1f-483f-b8fd-59c6632add77");
        }
    }
}