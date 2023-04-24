using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<DomainPackageModel> GetDomainPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(DomainPackageModel.SpecializationTypeId)
                .Select(x => new DomainPackageModel(x))
                .ToList();
        }

        public static bool IsDomainPackageModel(this IPackage package)
        {
            return package?.SpecializationTypeId == DomainPackageModel.SpecializationTypeId;
        }

        [IntentManaged(Mode.Ignore)]
        public static DomainPackageModel AsDomainPackageModel(this IPackage package)
        {
            if (package == null)
            {
                return null;
            }
            return new DomainPackageModel(package);
        }
    }
}