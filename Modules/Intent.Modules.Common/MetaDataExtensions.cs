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
        public static IEnumerable<IServiceModel> GetServiceModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = "Services")
        {
            return metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier(metaDataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IClass> GetDomainModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = "Domain")
        {
            return metaDataManager.GetMetaData<IClass>(new MetaDataIdentifier(metaDataIdentifier ?? "Domain")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
        
        public static IEnumerable<IDTOModel> GetDTOModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = "Services")
        {
            return metaDataManager.GetMetaData<IDTOModel>(new MetaDataIdentifier(metaDataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
        
        public static IEnumerable<IEnumModel> GetEnumModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier)
        {
            return metaDataManager.GetMetaData<IEnumModel>(new MetaDataIdentifier(metaDataIdentifier)).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
