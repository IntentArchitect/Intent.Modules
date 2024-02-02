using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
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

        public static IList<SendIntegrationCommandTargetEndModel> GetSentIntegrationCommands(this IEnumerable<IAssociation> associations)
        {
            var result = associations
                .Select(s => s.TargetEnd?.SpecializationType == SendIntegrationCommandTargetEndModel.SpecializationType ? s.TargetEnd : null)
                .Where(p => p is not null)
                .Select(x => SendIntegrationCommandModel.CreateFromEnd(x).TargetEnd)
                .ToList();
            return result;
        }

        public static IList<SendIntegrationCommandSourceEndModel> IntegrationCommandSources(this IntegrationCommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SendIntegrationCommandModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SendIntegrationCommandModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }
    }
}