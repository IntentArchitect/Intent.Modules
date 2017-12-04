using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.MetaModel.DTO;

namespace Intent.SoftwareFactory
{
    public static class IntentMetadataExtensions
    {
        public static IEnumerable<IServiceModel> GetServiceModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = null)
        {
            return metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier(metaDataIdentifier ?? "Service")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IClass> GetDomainModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = null)
        {
            return metaDataManager.GetMetaData<IClass>(new MetaDataIdentifier(metaDataIdentifier ?? "DomainEntity")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
        
        public static IEnumerable<IDTOModel> GetDTOModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metaDataIdentifier = null)
        {
            return metaDataManager.GetMetaData<IDTOModel>(new MetaDataIdentifier(metaDataIdentifier ?? "DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
