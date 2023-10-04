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
    public static class CreateEntityActionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CreateEntityActionTargetEndModel> CreateEntityActions(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CreateEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CreateEntityActionTargetEndModel> CreateEntityActions(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CreateEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CreateEntityActionSourceEndModel> CreateEntityCommands(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CreateEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CreateEntityActionSourceEndModel> CreateEntityCommands(this ClassConstructorModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CreateEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }
    }
}