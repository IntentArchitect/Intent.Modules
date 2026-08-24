using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<FileClassificationsModel> GetFileClassificationsModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FileClassificationsModel.SpecializationTypeId)
                .Select(x => new FileClassificationsModel(x))
                .ToList();
        }

    }
}