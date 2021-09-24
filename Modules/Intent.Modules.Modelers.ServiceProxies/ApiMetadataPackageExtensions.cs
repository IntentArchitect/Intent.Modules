using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.ServiceProxies.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<ServiceProxiesPackageModel> GetServiceProxiesPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(ServiceProxiesPackageModel.SpecializationTypeId)
                .Select(x => new ServiceProxiesPackageModel(x))
                .ToList();
        }


    }
}