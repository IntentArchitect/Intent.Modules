using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class UpdateEntityActionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<UpdateEntityActionTargetEndModel> UpdateEntityActions(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => UpdateEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpdateEntityActionSourceEndModel> UpdateEntityCommands(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => UpdateEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpdateEntityActionSourceEndModel> UpdateEntityCommands(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => UpdateEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}