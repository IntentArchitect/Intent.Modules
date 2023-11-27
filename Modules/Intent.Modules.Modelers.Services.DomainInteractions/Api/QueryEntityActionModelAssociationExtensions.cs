using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class QueryEntityActionModelAssociationExtensions
    {
        public static IList<QueryEntityActionTargetEndModel> QueryEntityActions(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == QueryEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => QueryEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        public static IList<QueryEntityActionSourceEndModel> QueryEntitySources(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == QueryEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => QueryEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}