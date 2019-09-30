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

        public IEnumerable<ITemplateDefinition> GetTemplateDefinitions()
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsTemplate()).ToList();
            var result = templates.Select(x => new TemplateDefinition(x, this)).ToList();
            return result;
        }

        public IEnumerable<ITemplateDefinition> GetTemplateDefinitions(string applicationId)
        {
            return GetTemplateDefinitions().Where(x => x.Application.Id == applicationId);
        }

        public IEnumerable<ITemplateDefinition> GetCSharpTemplates(IApplication application)
        {
            return GetTemplateDefinitions().Where(x => x.Type == ModuleBuilderElementType.CSharpTemplate && x.Application.Name == application.Name);
        }

        public IEnumerable<ITemplateDefinition> GetFileTemplates(IApplication application)
        {
            return GetTemplateDefinitions().Where(x => x.Type == ModuleBuilderElementType.FileTemplate && x.Application.Name == application.Name);
        }

        public IEnumerable<IDecoratorDefinition> GetDecorators()
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsDecorator()).ToList();
            var result = templates.Select(x => new DecoratorDefinition(x)).ToList();
            return result;
        }

        public IEnumerable<IDecoratorDefinition> GetDecorators(IApplication application)
        {
            return GetDecorators().Where(x => x.Application.Name == application.Name);
        }

        public IEnumerable<IModeler> GetModelers(IElementApplication application)
        {
            var modelerElements = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsModeler() && x.Application.Name == application.Name).ToList();
            var result = modelerElements.Select(x => new Modeler(x)).ToList();
            return result;
        }
    }
}