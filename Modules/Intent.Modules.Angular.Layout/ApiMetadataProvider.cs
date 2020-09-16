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

        public IList<FormModel> GetFormModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == FormModel.SpecializationType)
                .Select(x => new FormModel(x))
                .ToList<FormModel>();
            return models;
        }

        public IList<FormControlModel> GetFormControlModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Angular", application.Id)
                .Where(x => x.SpecializationType == FormControlModel.SpecializationType)
                .Select(x => new FormControlModel(x))
                .ToList<FormControlModel>();
            return models;
        }

    }
}