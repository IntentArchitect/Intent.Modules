using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class AssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<AssociationTargetEndModel> AssociatedToClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => AssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AssociationSourceEndModel> AssociatedFromClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => AssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}