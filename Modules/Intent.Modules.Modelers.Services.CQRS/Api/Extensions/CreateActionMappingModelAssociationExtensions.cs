using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    public static class CreateActionMappingModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CreateActionMappingTargetEndModel> CreatedEntities(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateActionMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CreateActionMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CreateActionMappingTargetEndModel> CreatedEntities(this QueryModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateActionMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CreateActionMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
        
        [IntentManaged(Mode.Fully)]
        public static IList<CreateActionMappingSourceEndModel> CreatedFrom(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateActionMappingModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CreateActionMappingModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CreateActionMappingEndModel> CreateActionMappingEnds(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsCreateActionMappingEndModel())
                .Select(CreateActionMappingEndModel.Create)
                .ToList();
        }

    }
}