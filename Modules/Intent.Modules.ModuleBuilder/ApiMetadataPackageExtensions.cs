using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<IntentDesignerPackageModel> GetIntentDesignerPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(IntentDesignerPackageModel.SpecializationTypeId)
                .Select(x => new IntentDesignerPackageModel(x))
                .ToList();
        }

        public static IList<IntentModuleModel> GetIntentModuleModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(IntentModuleModel.SpecializationTypeId)
                .Select(x => new IntentModuleModel(x))
                .ToList();
        }


    }
}