using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    public static class SendIntegrationCommandModelAssociationExtensions
    {
        public static IList<SendIntegrationCommandTargetEndModel> SentIntegrationCommands(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendIntegrationCommandModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SendIntegrationCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        public static IList<SendIntegrationCommandSourceEndModel> IntegrationCommandSources(this MessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendIntegrationCommandModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SendIntegrationCommandModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}