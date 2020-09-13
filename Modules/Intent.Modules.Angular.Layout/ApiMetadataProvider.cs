using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<PaginationModel> GetPaginationModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == PaginationModel.SpecializationType)
                .Select(x => new PaginationModel(x))
                .ToList<PaginationModel>();
            return models;
        }

        public IList<TableModel> GetTableModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == TableModel.SpecializationType)
                .Select(x => new TableModel(x))
                .ToList<TableModel>();
            return models;
        }

    }
}