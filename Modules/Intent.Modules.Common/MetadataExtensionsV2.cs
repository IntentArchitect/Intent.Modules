using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modules.Common
{
    public static class MetadataExtensionsV2
    {
        public static IEnumerable<IClass> GetClassModels(this IMetadataManager metaDataManager, IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IClass>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IEnumModel> GetEnumModels(this IMetadataManager metaDataManager, IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IEnumModel>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}