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

        public IEnumerable<IFileTemplate> GetTemplateDefinitions()
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsTemplate()).ToList();
            var result = templates.Select(x => new FileTemplate(x)).ToList();
            return result;
        }

        public IEnumerable<IFileTemplate> GetTemplateDefinitions(string applicationId)
        {
            return GetTemplateDefinitions().Where(x => x.Application.Id == applicationId);
        }

        public IEnumerable<IFileTemplate> GetCSharpTemplates(IApplication application)
        {
            return GetTemplateDefinitions().Where(x => x.Type == ModuleBuilderElementType.CSharpTemplate && x.Application.Name == application.Name);
        }

        public IEnumerable<IFileTemplate> GetFileTemplates(IApplication application)
        {
            return GetTemplateDefinitions().Where(x => x.Type == ModuleBuilderElementType.FileTemplate && x.Application.Name == application.Name);
        }

        public IEnumerable<IModulePackage> GetModulePackages(string applicationId)
        {
            var packages = _metadataManager.GetMetadata<IElement>("Module Builder")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ModulePackage.SpecializationType)
                .Select(x => new ModulePackage(x)).ToList();
            return packages;
        }

        public IEnumerable<IModuleStereotype> GetModuleStereotypes(string applicationId)
        {
            var results = _metadataManager.GetMetadata<IElement>("Module Builder")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ModuleStereotype.SpecializationType)
                .Select(x => new ModuleStereotype(x)).ToList();
            return results;
        }

        public IEnumerable<IModulePackageFolder> GetModulePackageFolders(string applicationId)
        {
            var results = _metadataManager.GetMetadata<IElement>("Module Builder")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ModulePackageFolder.SpecializationType)
                .Select(x => new ModulePackageFolder(x)).ToList();
            return results;
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

        public IEnumerable<IModelerReference> GetModelers(IElementApplication application)
        {
            var modelerElements = _metadataManager.GetMetadata<IElement>("Module Builder").Where(x => x.IsModeler() && x.Application.Name == application.Name).ToList();
            var result = modelerElements.Select(x => new ModelerReference(x)).ToList();
            return result;
        }
    }
}