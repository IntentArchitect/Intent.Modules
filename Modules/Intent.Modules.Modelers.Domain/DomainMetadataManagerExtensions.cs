using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;

namespace Intent.Modelers.Domain
{
    public static class DomainMetadataManagerExtensions
    {
        public static IEnumerable<IClass> GetDomainClasses(this IMetadataManager metadataManager, IApplication application)
        {
            return new DomainMetadataProvider(metadataManager).GetClasses(application);
        }
    }
}