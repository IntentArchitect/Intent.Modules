using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Angular.Api;

namespace Intent.Modules.Angular
{
    public static class MetadataManagerExtensions
    {
        public static IEnumerable<IModuleModel> GetModules(this IMetadataManager metadataManager, IApplication application)
        {
            return new MetadataProvider(metadataManager).GetClasses(application);
        }
    }
}