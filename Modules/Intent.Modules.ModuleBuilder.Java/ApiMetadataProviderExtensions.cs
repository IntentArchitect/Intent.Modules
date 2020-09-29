using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Java.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<JavaFileTemplateModel> GetJavaFileTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(JavaFileTemplateModel.SpecializationTypeId)
                .Select(x => new JavaFileTemplateModel(x))
                .ToList();
        }

    }
}