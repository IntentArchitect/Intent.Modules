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
    public static class DeleteEntityActionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DeleteEntityActionTargetEndModel> DeleteEntityActions(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DeleteEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DeleteEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DeleteEntityActionTargetEndModel> DeleteEntityActions(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DeleteEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DeleteEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DeleteEntityActionSourceEndModel> DeleteEntityCommands(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DeleteEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DeleteEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}