using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    public static class AggregationModelExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<AggregationEndModel> AggregatedFromClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AggregationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => AggregationModel.CreateFromEnd(x))
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AggregationEndModel> AggregatedToClasses(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AggregationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => AggregationModel.CreateFromEnd(x))
                .ToList();
        }
    }

}