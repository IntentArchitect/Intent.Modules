using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Modelers.Services
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ControllerModel> GetControllerModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetControllerModels(application);
        }

        public static IList<DTOModel> GetDTOModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDTOModels(application);
        }

        public static IList<EnumModel> GetEnumModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetEnumModels(application);
        }

        public static IList<FolderModel> GetFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolderModels(application);
        }

        public static IList<ServiceModel> GetServiceModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetServiceModels(application);
        }

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTypeDefinitionModels(application);
        }

    }
}