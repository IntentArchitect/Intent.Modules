using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IEnumerable<IDTOModel> GetAllDTOs(this IMetadataManager metadataManager)
        {
            return new ApiMetadataProvider(metadataManager).GetAllDTOs();
        }

        public static IEnumerable<IDTOModel> GetDTOs(this IMetadataManager metadataManager, string applicationId)
        {
            return new ApiMetadataProvider(metadataManager).GetDTOs(applicationId);
        }

        public static IEnumerable<IServiceModel> GetAllServices(this IMetadataManager metadataManager)
        {
            return new ApiMetadataProvider(metadataManager).GetAllServices();
        }

        public static IEnumerable<IServiceModel> GetServices(this IMetadataManager metadataManager, string applicationId)
        {
            return new ApiMetadataProvider(metadataManager).GetServices(applicationId);
        }

        public static IEnumerable<IEnumModel> GetAllEnums(this IMetadataManager metadataManager)
        {
            return new ApiMetadataProvider(metadataManager).GetAllEnums();
        }

        public static IEnumerable<IEnumModel> GetEnums(this IMetadataManager metadataManager, string applicationId)
        {
            return new ApiMetadataProvider(metadataManager).GetEnums(applicationId);
        }

        public static bool IsDTO(this IElement model)
        {
            return model.SpecializationType.Equals("DTO", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsService(this IElement model)
        {
            return model.SpecializationType.Equals("Service", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsEnum(this IElement model)
        {
            return model.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}