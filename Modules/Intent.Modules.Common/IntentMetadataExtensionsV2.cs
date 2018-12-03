using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Common
{
    public static class IntentMetadataExtensionsV2
    {
        public static IEnumerable<IClass> GetClassModels(this IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IClass>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IEnumModel> GetEnumModels(this IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IEnumModel>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}