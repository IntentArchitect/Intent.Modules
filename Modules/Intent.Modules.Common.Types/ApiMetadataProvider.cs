using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.Common.Types.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<FolderModel> GetFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Common Types", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<TypeDefinitionModel> GetTypeDefinitionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Common Types", application.Id)
                .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
                .Select(x => new TypeDefinitionModel(x))
                .ToList<TypeDefinitionModel>();
            return models;
        }

    }
}