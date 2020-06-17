using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class GeneralizationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationTargetEndModel> Generalizations(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationSourceEndModel> Specializations(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}