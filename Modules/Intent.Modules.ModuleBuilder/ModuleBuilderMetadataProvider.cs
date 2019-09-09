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

        public IEnumerable<ICSharpTemplate> GetCSharpTemplates()
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsCSharpTemplate()).ToList();
            var result = templates.Select(x => new CSharpTemplate(x)).ToList();
            return result;
        }

        public IEnumerable<ICSharpTemplate> GetCSharpTemplates(IApplication application)
        {
            return GetCSharpTemplates().Where(x => x.Application.Name == application.ApplicationName);
        }

        public IEnumerable<IFileTemplate> GetFileTemplates()
        {
            var classes = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsFileTemplate()).ToList();
            var result = classes.Select(x => new FileTemplate(x)).ToList();
            return result;
        }

        public IEnumerable<IFileTemplate> GetFileTemplates(IApplication application)
        {
            return GetFileTemplates().Where(x => x.Application.Name == application.ApplicationName);
        }

        public IEnumerable<IDecoratorDefinition> GetDecorators()
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsDecorator()).ToList();
            var result = templates.Select(x => new DecoratorDefinition(x)).ToList();
            return result;
        }

        public IEnumerable<IDecoratorDefinition> GetDecorators(IApplication application)
        {
            return GetDecorators().Where(x => x.Application.Name == application.ApplicationName);
        }

        public IEnumerable<IModuleBuilderElement> GetAllElements()
        {
            return GetCSharpTemplates().Cast<IModuleBuilderElement>().Concat(GetFileTemplates()).Concat(GetDecorators());
        }

        public IEnumerable<IModuleBuilderElement> GetAllElements(IApplication application)
        {
            return GetAllElements().Where(x => x.Application.Name == application.ApplicationName);
        }
    }
}