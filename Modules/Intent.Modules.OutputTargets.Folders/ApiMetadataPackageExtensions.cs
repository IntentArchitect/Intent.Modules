using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<RootLocationPackageModel> GetRootLocationPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(RootLocationPackageModel.SpecializationTypeId)
                .Select(x => new RootLocationPackageModel(x))
                .ToList();
        }


    }
}