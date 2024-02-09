using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class NavigationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<NavigationTargetEndModel> NavigateToComponents(this ComponentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == NavigationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => NavigationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<NavigationSourceEndModel> NavigateBackComponents(this ComponentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == NavigationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => NavigationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<NavigationEndModel> NavigationEnds(this ComponentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsNavigationEndModel())
                .Select(NavigationEndModel.Create)
                .ToList();
        }

    }
}