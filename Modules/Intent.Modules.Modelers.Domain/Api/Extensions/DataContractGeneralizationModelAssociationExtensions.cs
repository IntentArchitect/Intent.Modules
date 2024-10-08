using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class DataContractGeneralizationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DataContractGeneralizationTargetEndModel> Generalizations(this DataContractModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DataContractGeneralizationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DataContractGeneralizationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DataContractGeneralizationSourceEndModel> Specializations(this DataContractModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DataContractGeneralizationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DataContractGeneralizationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DataContractGeneralizationEndModel> DataContractGeneralizationEnds(this DataContractModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsDataContractGeneralizationEndModel())
                .Select(DataContractGeneralizationEndModel.Create)
                .ToList();
        }

    }
}