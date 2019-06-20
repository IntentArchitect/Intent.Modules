using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modules.Common
{
    public static class MetadataExtensionsV2
    {
        public static IEnumerable<IElement> GetClassModels(this IMetadataManager metaDataManager, Intent.Engine.IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IElement>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        //public static IEnumerable<IEnumModel> GetEnumModels(this IMetadataManager metaDataManager, IApplication application, string metadataIdentifier)
        //{
        //    return metaDataManager.GetMetaData<IEnumModel>(metadataIdentifier).Where(x => x.Application.Name == application.ApplicationName).ToList();
        //}
    }

    public class CodeGenType
    {
        public const string Basic = "Basci";
        public const string UserControlledWeave = "UserControlledWeave";
    }
}