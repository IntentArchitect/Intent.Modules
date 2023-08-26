using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Api
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
        public static IList<CreateActionMappingSourceEndModel> CreatedFrom(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CreateActionMappingModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CreateActionMappingModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}