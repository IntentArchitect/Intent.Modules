using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace ModuleBuilders.Api
{
    public static class NewAssociationTypeModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<NewAssociationTypeTargetEndModel> NewAssociationTypeTargets(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == NewAssociationTypeModel.SpecializationType && x.IsTargetEnd())
                .Select(x => NewAssociationTypeModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<NewAssociationTypeSourceEndModel> NewAssociationTypeSources(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == NewAssociationTypeModel.SpecializationType && x.IsSourceEnd())
                .Select(x => NewAssociationTypeModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<NewAssociationTypeEndModel> NewAssociationTypeEnds(this ClassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsNewAssociationTypeEndModel())
                .Select(NewAssociationTypeEndModel.Create)
                .ToList();
        }

    }
}