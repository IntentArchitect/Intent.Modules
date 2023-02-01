using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Dart.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<DartFileTemplateModel> GetDartFileTemplateModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DartFileTemplateModel.SpecializationTypeId)
                .Select(x => new DartFileTemplateModel(x))
                .ToList();
        }

    }
}