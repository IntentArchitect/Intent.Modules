using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    public static class DeleteEntityActionModelAssociationExtensions
    {
        public static IList<DeleteEntityActionTargetEndModel> DeleteEntityActions(this IProcessingHandlerModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DeleteEntityActionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DeleteEntityActionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }
        
        public static IList<DeleteEntityActionSourceEndModel> DeleteEntityCommands(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DeleteEntityActionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DeleteEntityActionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}