using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CardModel> GetCardModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CardModel.SpecializationTypeId)
                .Select(x => new CardModel(x))
                .ToList();
        }
        public static IList<DialogModel> GetDialogModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DialogModel.SpecializationTypeId)
                .Select(x => new DialogModel(x))
                .ToList();
        }

    }
}