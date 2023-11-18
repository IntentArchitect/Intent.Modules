using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class SendCommandModelAssociationExtensions
    {
        public static IList<SendCommandTargetEndModel> SentCommandDestinations(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendCommandModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SendCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
    }
}