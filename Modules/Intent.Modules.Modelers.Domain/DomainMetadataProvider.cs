using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;

namespace Intent.Modelers.Domain
{
    public class DomainMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public DomainMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IClass> GetClasses()
        {
            var cache = new Dictionary<string, Class>();
            var classes = _metadataManager.GetMetadata<Metadata.Models.IElement>("Domain").Where(x => x.SpecializationType == "Class").ToList();
            var result = classes.Select(x => cache.ContainsKey(x.Id) ? cache[x.Id] : new Class(x, cache)).ToList();
            return result;
        }

        public IEnumerable<IClass> GetClasses(IApplication application)
        {
            return GetClasses().Where(x => x.Application.Name == application.Name);
        }
    }
}