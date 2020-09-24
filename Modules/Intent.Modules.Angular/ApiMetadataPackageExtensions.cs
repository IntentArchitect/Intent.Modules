using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<AngularAppModel> GetAngularAppModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(AngularAppModel.SpecializationTypeId)
                .Select(x => new AngularAppModel(x))
                .ToList();
        }


    }
}