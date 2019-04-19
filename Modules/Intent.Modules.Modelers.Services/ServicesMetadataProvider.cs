using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modelers.Services
{
    public class ServicesMetadataProvider
    {
        private readonly IMetaDataManager _metaDataManager;

        public ServicesMetadataProvider(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public IEnumerable<IDTOModel> GetAllDTOs()
        {
            var classes = _metaDataManager.GetMetaData<IClass>("Services").Where(x => x.SpecializationType == "DTO").ToList();
            return classes.Select(x => new DTOModel(x)).ToList();
        }

        public IEnumerable<IDTOModel> GetDTOs(SoftwareFactory.Engine.IApplication application)
        {
            return GetAllDTOs().Where(x => x.Application.Name == application.ApplicationName);
        }
    }
}