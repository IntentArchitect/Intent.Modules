using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<TypescriptFileTemplateModel> GetTypescriptFileTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TypescriptFileTemplateModel.SpecializationTypeId)
                .Select(x => new TypescriptFileTemplateModel(x))
                .ToList();
        }

    }
}