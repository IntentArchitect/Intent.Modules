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
        public static IList<ApiVersionModel> GetApiVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ApiVersionModel.SpecializationTypeId)
                .Select(x => new ApiVersionModel(x))
                .ToList();
        }

    }
}