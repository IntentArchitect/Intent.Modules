using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    public static class GeneralizationModelExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationEndModel> Specializations(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x))
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationEndModel> Generalizations(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x))
                .ToList();
        }
    }

}