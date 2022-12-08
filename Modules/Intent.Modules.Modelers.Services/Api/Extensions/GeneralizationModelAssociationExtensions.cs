using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    public static class GeneralizationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationTargetEndModel> Generalizations(this DTOModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationSourceEndModel> Specializations(this DTOModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == GeneralizationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => GeneralizationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<GeneralizationEndModel> GeneralizationEnds(this DTOModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsGeneralizationEndModel())
                .Select(GeneralizationEndModel.Create)
                .ToList();
        }

    }
}