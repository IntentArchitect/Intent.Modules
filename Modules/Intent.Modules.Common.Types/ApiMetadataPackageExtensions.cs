using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.Common.Types
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