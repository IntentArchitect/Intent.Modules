using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.CSharp.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<CSharpTemplateModel> GetCSharpTemplateModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == CSharpTemplateModel.SpecializationType)
                .Select(x => new CSharpTemplateModel(x))
                .ToList<CSharpTemplateModel>();
            return models;
        }

    }
}