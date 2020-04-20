using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modelers.Domain.Api
{
    public static class DomainMetadataManagerExtensions
    {
        public static IEnumerable<ClassModel> GetDomainClasses(this IMetadataManager metadataManager, string applicationId)
        {
            return new DomainMetadataProvider(metadataManager).GetClasses(applicationId);
        }

        public static IEnumerable<IEnum> GetEnums(this IMetadataManager metadataManager, string applicationId)
        {
            return new DomainMetadataProvider(metadataManager).GetEnums(applicationId);
        }

        public static IEnumerable<ITypeDefinition> GetTypeDefinitions(this IMetadataManager metadataManager, string applicationId)
        {
            return new DomainMetadataProvider(metadataManager).GetTypeDefinitions(applicationId);
        }
    }
}