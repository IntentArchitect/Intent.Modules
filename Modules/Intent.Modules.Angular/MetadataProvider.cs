using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Api;

namespace Intent.Modules.Angular
{
    public class MetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public MetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IModuleModel> GetModules()
        {
            var classes = _metadataManager.GetMetadata<Metadata.Models.IElement>("Angular").Where(x => x.SpecializationType == "Module").ToList();
            return classes.Select(x => new ModuleModel(x)).ToList();
        }

        public IEnumerable<IModuleModel> GetModules(IApplication application)
        {
            return GetModules().Where(x => x.Application.Name == application.ApplicationName);
        }
    }
}