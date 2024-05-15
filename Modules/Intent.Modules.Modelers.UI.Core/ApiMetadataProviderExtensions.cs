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
        public static IList<IconModel> GetIconModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(IconModel.SpecializationTypeId)
                .Select(x => new IconModel(x))
                .ToList();
        }
        public static IList<LinkModel> GetLinkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(LinkModel.SpecializationTypeId)
                .Select(x => new LinkModel(x))
                .ToList();
        }

        public static IList<SelectModel> GetSelectModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SelectModel.SpecializationTypeId)
                .Select(x => new SelectModel(x))
                .ToList();
        }

    }
}