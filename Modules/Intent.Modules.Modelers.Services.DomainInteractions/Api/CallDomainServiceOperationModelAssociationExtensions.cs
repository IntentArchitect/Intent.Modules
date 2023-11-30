using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class CallDomainServiceOperationModelAssociationExtensions
    {
        public static IList<CallDomainServiceOperationTargetEndModel> CallDomainServiceOperationTargets(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallDomainServiceOperationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CallDomainServiceOperationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        public static IList<CallDomainServiceOperationSourceEndModel> CallDomainServiceOperationSources(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CallDomainServiceOperationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CallDomainServiceOperationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}