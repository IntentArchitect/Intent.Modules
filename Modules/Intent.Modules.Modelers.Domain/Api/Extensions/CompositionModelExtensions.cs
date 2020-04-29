using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    public static class CompositionModelExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<CompositionEndModel> ComposedFromClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CompositionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => CompositionModel.CreateFromEnd(x))
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<CompositionEndModel> ComposedToClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == CompositionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => CompositionModel.CreateFromEnd(x))
                .ToList();
        }
    }

}