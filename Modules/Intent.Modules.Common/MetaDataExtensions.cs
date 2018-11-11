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
            var result = metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier(metadataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
            if (result.Any())
            {
                return result;
            }

            // Purely for backward compatibility between 1.5.x and 1.6.x
            return metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier("Service")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IClass> GetDomainModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = "Domain")
        {
            var result = metaDataManager.GetMetaData<IClass>(new MetaDataIdentifier(metadataIdentifier ?? "Domain")).Where(x => x.Application.Name == application.ApplicationName).ToList();
            if (result.Any())
            {
                return result;
            }

            // Purely for backward compatibility between 1.5.x and 1.6.x
            return metaDataManager.GetMetaData<IClass>(new MetaDataIdentifier("DomainEntity")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IDTOModel> GetDTOModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = "Services")
        {
            var result = metaDataManager.GetMetaData<IDTOModel>(new MetaDataIdentifier(metadataIdentifier ?? "Services")).Where(x => x.Application.Name == application.ApplicationName).ToList();
            if (result.Any())
            {
                return result;
            }

            // Purely for backward compatibility between 1.5.x and 1.6.x
            return metaDataManager.GetMetaData<IDTOModel>(new MetaDataIdentifier("DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }

        public static IEnumerable<IEnumModel> GetEnumModels(this Engine.IMetaDataManager metaDataManager, Engine.IApplication application, string metadataIdentifier = null)
        {
            if (!string.IsNullOrWhiteSpace(metadataIdentifier))
            {
                return metaDataManager.GetMetaData<IEnumModel>(new MetaDataIdentifier(metadataIdentifier)).Where(x => x.Application.Name == application.ApplicationName).ToList();
            }

            return new IEnumModel[0]
                .Concat(metaDataManager.GetMetaData<IEnumModel>(new MetaDataIdentifier("Domain")).Where(x => x.Application.Name == application.ApplicationName))
                .Concat(metaDataManager.GetMetaData<IEnumModel>(new MetaDataIdentifier("Services")).Where(x => x.Application.Name == application.ApplicationName))
                .ToList();
        }
    }
}
