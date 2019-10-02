using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public class ServicesMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ServicesMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IDTOModel> GetAllDTOs()
        {
            var classes = _metadataManager.GetMetadata<IElement>("Services").Where(x => x.IsDTO()).ToList();
            return classes.Select(x => new DTOModel(x)).ToList();
        }

        public IEnumerable<IDTOModel> GetDTOs(IApplication application)
        {
            return GetAllDTOs().Where(x => x.Application.Name == application.Name);
        }

        public IEnumerable<IServiceModel> GetServices(IApplication application)
        {
            var classes = _metadataManager.GetMetadata<IElement>("Services").Where(x => x.Application.Name == application.Name
                && x.IsService()).ToList();
            return classes.Select(x => new ServiceModel(x)).ToList();
        }
    }

    public static class ServicesMetadataManagerExtensions
    {
        public static IEnumerable<IDTOModel> GetDTOs(this IMetadataManager metadataManager, IApplication application)
        {
            return new ServicesMetadataProvider(metadataManager).GetDTOs(application);
        }

        public static IEnumerable<IServiceModel> GetServices(this IMetadataManager metadataManager, IApplication application)
        {
            return new ServicesMetadataProvider(metadataManager).GetServices(application);
        }

        public static bool IsDTO(this IElement model)
        {
            return model.SpecializationType == "DTO";
        }

        public static bool IsService(this IElement model)
        {
            return model.SpecializationType == "Service";
        }

        public static bool IsEnum(this IElement model)
        {
            return model.SpecializationType == "Enum";
        }
    }
}