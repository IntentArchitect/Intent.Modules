using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<ComponentModel> GetComponentModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == ComponentModel.SpecializationType)
                .Select(x => new ComponentModel(x))
                .ToList<ComponentModel>();
            return models;
        }

        public IList<EnumModel> GetEnumModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == EnumModel.SpecializationType)
                .Select(x => new EnumModel(x))
                .ToList<EnumModel>();
            return models;
        }

        public IList<FolderModel> GetFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<ModelDefinitionModel> GetModelDefinitionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == ModelDefinitionModel.SpecializationType)
                .Select(x => new ModelDefinitionModel(x))
                .ToList<ModelDefinitionModel>();
            return models;
        }

        public IList<ModuleModel> GetModuleModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == ModuleModel.SpecializationType)
                .Select(x => new ModuleModel(x))
                .ToList<ModuleModel>();
            return models;
        }

        public IList<ServiceProxyModel> GetServiceProxyModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == ServiceProxyModel.SpecializationType)
                .Select(x => new ServiceProxyModel(x))
                .ToList<ServiceProxyModel>();
            return models;
        }

        public IList<TypeDefinitionModel> GetTypeDefinitionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
                .Select(x => new TypeDefinitionModel(x))
                .ToList<TypeDefinitionModel>();
            return models;
        }

    }
}