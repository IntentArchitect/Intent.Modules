using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Api;

namespace Intent.Modules.Angular
{
    public class MetadataProvider
    {
        private readonly IMetadataManager _metaDataManager;

        public MetadataProvider(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public IEnumerable<IModuleModel> GetClasses()
        {
            var classes = _metaDataManager.GetMetaData<Metadata.Models.IClass>("Angular").Where(x => x.SpecializationType == "Module").ToList();
            return classes.Select(x => new ModuleModel(x)).ToList();
        }

        public IEnumerable<IModuleModel> GetClasses(IApplication application)
        {
            return GetClasses().Where(x => x.Application.Name == application.ApplicationName);
        }
    }
}