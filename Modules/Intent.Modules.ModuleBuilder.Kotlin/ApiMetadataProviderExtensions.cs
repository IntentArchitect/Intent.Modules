using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Kotlin.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<KotlinFileTemplateModel> GetKotlinFileTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(KotlinFileTemplateModel.SpecializationTypeId)
                .Select(x => new KotlinFileTemplateModel(x))
                .ToList();
        }

    }
}