using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<RootFolderModel> GetRootFolderModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(RootFolderModel.SpecializationTypeId)
                .Select(x => new RootFolderModel(x))
                .ToList();
        }

        public static bool IsRootFolderModel(this IPackage package)
        {
            return package?.SpecializationTypeId == RootFolderModel.SpecializationTypeId;
        }


    }
}