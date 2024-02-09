using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<UserInterfacePackageModel> GetUserInterfacePackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(UserInterfacePackageModel.SpecializationTypeId)
                .Select(x => new UserInterfacePackageModel(x))
                .ToList();
        }

        public static bool IsUserInterfacePackageModel(this IPackage package)
        {
            return package?.SpecializationTypeId == UserInterfacePackageModel.SpecializationTypeId;
        }


    }
}