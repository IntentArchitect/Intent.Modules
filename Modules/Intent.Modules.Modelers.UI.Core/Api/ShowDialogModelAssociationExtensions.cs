using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modelers.UI.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class ShowDialogModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ShowDialogTargetEndModel> ShowDialogTargets(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ShowDialogModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ShowDialogModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ShowDialogSourceEndModel> ShowDialogSources(this ComponentModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ShowDialogModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ShowDialogModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}