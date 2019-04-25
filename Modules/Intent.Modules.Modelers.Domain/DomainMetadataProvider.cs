using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;

namespace Intent.Modelers.Domain
{
    public class DomainMetadataProvider
    {
        private readonly IMetadataManager _metaDataManager;

        public DomainMetadataProvider(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public IEnumerable<IClass> GetClasses()
        {
            var classes = _metaDataManager.GetMetaData<Metadata.Models.IClass>("Domain").Where(x => x.SpecializationType == "Class").ToList();
            return classes.Select(x => new Class(x)).ToList();
        }

        public IEnumerable<IClass> GetClasses(IApplication application)
        {
            return GetClasses().Where(x => x.Application.Name == application.ApplicationName);
        }
    }
}