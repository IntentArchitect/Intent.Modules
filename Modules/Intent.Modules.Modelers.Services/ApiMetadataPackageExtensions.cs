using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<PackageModel> GetPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(PackageModel.SpecializationTypeId)
                .Select(x => new PackageModel(x))
                .ToList();
        }


    }
}