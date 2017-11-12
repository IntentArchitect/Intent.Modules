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
        public static IEnumerable<IServiceModel> GetServiceModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application)
        {
            return metaDataManager.GetMetaData<IServiceModel>(new MetaDataType("Service")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IClass> GetDomainModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application)
        {
            return metaDataManager.GetMetaData<IClass>(new MetaDataType("DomainEntity")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IDTOModel> GetDTOModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application)
        {
            return metaDataManager.GetMetaData<IDTOModel>(new MetaDataType("DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
