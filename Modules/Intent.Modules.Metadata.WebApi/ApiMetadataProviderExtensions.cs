using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<VersionDefinitionModel> GetVersionDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(VersionDefinitionModel.SpecializationTypeId)
                .Select(x => new VersionDefinitionModel(x))
                .ToList();
        }

    }
}