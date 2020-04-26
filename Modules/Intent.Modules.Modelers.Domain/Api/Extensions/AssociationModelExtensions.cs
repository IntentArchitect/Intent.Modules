using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    public static class AssociationModelExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<AssociationEndModel> AssociatedFromClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => AssociationModel.CreateFromEnd(x))
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AssociationEndModel> AssociatedToClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => AssociationModel.CreateFromEnd(x))
                .ToList();
        }
    }

}