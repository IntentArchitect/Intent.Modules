using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Api
{
    public static class UpdateActionMappingModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<UpdateActionMappingTargetEndModel> UpdatedEntities(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateActionMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => UpdateActionMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpdateActionMappingTargetEndModel> UpdatedEntities(this QueryModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateActionMappingModel.SpecializationType && x.IsTargetEnd())
                .Select(x => UpdateActionMappingModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpdateActionMappingSourceEndModel> UpdatedFrom(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UpdateActionMappingModel.SpecializationType && x.IsSourceEnd())
                .Select(x => UpdateActionMappingModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}