using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace ModuleBuilders.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<TestElementModel> GetTestElementModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TestElementModel.SpecializationTypeId)
                .Select(x => new TestElementModel(x))
                .ToList();
        }

    }
}