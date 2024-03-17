using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<IntentModuleModel> GetIntentModuleModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(IntentModuleModel.SpecializationTypeId)
                .Select(x => new IntentModuleModel(x))
                .ToList();
        }

        public static bool IsIntentModuleModel(this IPackage package)
        {
            return package?.SpecializationTypeId == IntentModuleModel.SpecializationTypeId;
        }


    }
}