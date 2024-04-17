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
        public static IList<LinkModel> GetLinkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(LinkModel.SpecializationTypeId)
                .Select(x => new LinkModel(x))
                .ToList();
        }

    }
}