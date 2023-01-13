using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    public static class ResourceAccessModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAccessTargetEndModel> AssociationTargetEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAccessModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ResourceAccessModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAccessSourceEndModel> AssociationSourceEnd(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAccessModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ResourceAccessModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAccessEndModel> ResourceAccessEnds(this PlaceholderModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsResourceAccessEndModel())
                .Select(ResourceAccessEndModel.Create)
                .ToList();
        }

    }
}