using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder
{
    public class ModuleBuilderMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ModuleBuilderMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IFileTemplate> GetTemplateDefinitions(string applicationId)
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId)
                .Where(x => x.TypeReference?.Element.SpecializationType == TemplateRegistration.SpecializationType).ToList();
            var result = templates.Select(x => new FileTemplate(x)).ToList();
            return result; 
        } 
    }
}