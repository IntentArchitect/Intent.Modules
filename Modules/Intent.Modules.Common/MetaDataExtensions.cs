using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.MetaModel.DTO;
using Intent.MetaModel.Enums;

namespace Intent.SoftwareFactory
{
    public static class IntentMetadataExtensions
    {
        public static IEnumerable<IServiceModel> GetServiceModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = "Services")
        {
            return metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier(metadataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IClass> GetDomainModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = "Domain")
        {
            return metaDataManager.GetMetaData<IClass>(new MetaDataIdentifier(metadataIdentifier ?? "Domain")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
        
        public static IEnumerable<IDTOModel> GetDTOModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = "Services")
        {
            return metaDataManager.GetMetaData<IDTOModel>(new MetaDataIdentifier(metadataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
        
        public static IEnumerable<IEnumModel> GetEnumModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier)
        {
            return metaDataManager.GetMetaData<IEnumModel>(new MetaDataIdentifier(metadataIdentifier)).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
