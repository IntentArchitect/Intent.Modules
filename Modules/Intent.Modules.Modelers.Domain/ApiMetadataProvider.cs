using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<ClassModel> GetClassModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == ClassModel.SpecializationType)
                .Select(x => new ClassModel(x))
                .ToList<ClassModel>();
            return models;
        }

        public IList<CommentModel> GetCommentModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == CommentModel.SpecializationType)
                .Select(x => new CommentModel(x))
                .ToList<CommentModel>();
            return models;
        }

        public IList<DiagramModel> GetDiagramModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == DiagramModel.SpecializationType)
                .Select(x => new DiagramModel(x))
                .ToList<DiagramModel>();
            return models;
        }

        public IList<EnumModel> GetEnumModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == EnumModel.SpecializationType)
                .Select(x => new EnumModel(x))
                .ToList<EnumModel>();
            return models;
        }

        public IList<FolderModel> GetFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<TypeDefinitionModel> GetTypeDefinitionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Domain", application.Id)
                .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
                .Select(x => new TypeDefinitionModel(x))
                .ToList<TypeDefinitionModel>();
            return models;
        }

    }
}