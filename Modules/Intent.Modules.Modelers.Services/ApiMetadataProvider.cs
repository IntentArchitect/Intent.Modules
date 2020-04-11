using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IDTOModel> GetAllDTOs()
        {
            var classes = _metadataManager.GetMetadata<IElement>("Services").Where(x => x.IsDTO()).ToList();
            return classes.Select(x => new DTOModel(x)).ToList();
        }

        public IEnumerable<IDTOModel> GetDTOs(string applicationId)
        {
            return GetAllDTOs().Where(x => x.Application.Id == applicationId);
        }

        public IEnumerable<IServiceModel> GetAllServices()
        {
            var classes = _metadataManager.GetMetadata<IElement>("Services").Where(x => x.IsService()).ToList();
            return classes.Select(x => new ServiceModel(x)).ToList();
        }

        public IEnumerable<IServiceModel> GetServices(string applicationId)
        {
            var classes = _metadataManager.GetMetadata<IElement>("Services").Where(x => x.Application.Id == applicationId
                && x.IsService()).ToList();
            return classes.Select(x => new ServiceModel(x)).ToList();
        }

        public IEnumerable<IEnumModel> GetAllEnums()
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Services").Where(x => x.IsEnum()).ToList();
            return types.Select(x => new EnumModel(x)).ToList();
        }

        public IEnumerable<IEnumModel> GetEnums(string applicationId)
        {
            return GetAllEnums().Where(x => x.Application.Id == applicationId);
        }
    }
}