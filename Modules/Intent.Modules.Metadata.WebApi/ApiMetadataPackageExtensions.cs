using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<OpenAPIPackageModel> GetOpenAPIPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(OpenAPIPackageModel.SpecializationTypeId)
                .Select(x => new OpenAPIPackageModel(x))
                .ToList();
        }

        public static bool IsOpenAPIPackageModel(this IPackage package)
        {
            return package?.SpecializationTypeId == OpenAPIPackageModel.SpecializationTypeId;
        }


    }
}