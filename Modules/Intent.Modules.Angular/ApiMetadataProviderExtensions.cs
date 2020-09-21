using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<AngularServiceModel> GetAngularServiceModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAngularServiceModels(application);
        }

        public static IList<ComponentModel> GetComponentModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetComponentModels(application);
        }

        public static IList<EnumModel> GetEnumModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetEnumModels(application);
        }

        public static IList<FolderModel> GetFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolderModels(application);
        }

        public static IList<FormGroupDefinitionModel> GetFormGroupDefinitionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFormGroupDefinitionModels(application);
        }

        public static IList<ModelDefinitionModel> GetModelDefinitionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelDefinitionModels(application);
        }

        public static IList<ModuleModel> GetModuleModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModuleModels(application);
        }

        public static IList<ServiceProxyModel> GetServiceProxyModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetServiceProxyModels(application);
        }

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTypeDefinitionModels(application);
        }

    }
}