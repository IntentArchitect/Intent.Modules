using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    public static class AssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<AssociationTargetEndModel> AssociationTargetEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => AssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AssociationSourceEndModel> AssociationSourceEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => AssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AssociationEndModel> AssociationEnds(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsAssociationEndModel())
                .Select(AssociationEndModel.Create)
                .ToList();
        }

    }
}