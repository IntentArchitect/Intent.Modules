using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class QueryEntityActionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<QueryEntityActionTargetEndModel> QueryEntityActions(this QueryModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == QueryEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => QueryEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<QueryEntityActionTargetEndModel> QueryEntityActions(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == QueryEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => QueryEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<QueryEntityActionSourceEndModel> QueryEntitySources(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == QueryEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => QueryEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}