using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modelers.Domain.Api
{
    public static class DomainMetadataManagerExtensions
    {
        public static IEnumerable<IClass> GetDomainClasses(this IMetadataManager metadataManager, IApplication application)
        {
            return new DomainMetadataProvider(metadataManager).GetClasses(application);
        }
    }
}