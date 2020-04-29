using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    public static class CommentAssociationModelExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CommentAssociationEndModel> AssociatedComments(this CommentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CommentAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CommentAssociationModel.CreateFromEnd(x))
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CommentAssociationEndModel> CommentedClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CommentAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CommentAssociationModel.CreateFromEnd(x))
                .ToList();
        }
    }

}