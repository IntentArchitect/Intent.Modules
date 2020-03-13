using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Api.Modeler;

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
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId).Where(x => x.IsTemplate()).ToList();
            var result = templates.Select(x => new FileTemplate(x)).ToList();
            return result;
        }

        public IEnumerable<ICSharpTemplate> GetCSharpTemplates(IApplication application)
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id).Where(x => x.IsCSharpTemplate()).ToList();
            var result = templates.Select(x => new CSharpTemplate(x)).ToList();
            return result;
        }

        public IEnumerable<IFileTemplate> GetFileTemplates(IApplication application)
        {
            return GetTemplateDefinitions(application.Id).Where(x => x.Type == ModuleBuilderElementType.FileTemplate && x.Application.Name == application.Name);
        }

        public IEnumerable<IModulePackage> GetModulePackages(string applicationId)
        {
            var packages = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId)
                .Where(x => x.SpecializationType == ModulePackage.SpecializationType)
                .Select(x => new ModulePackage(x)).ToList();
            return packages;
        }

        public IEnumerable<IModuleStereotype> GetModuleStereotypes(string applicationId)
        {
            var results = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId)
                .Where(x => x.SpecializationType == ModuleStereotype.SpecializationType)
                .Select(x => new ModuleStereotype(x)).ToList();
            return results;
        }

        public IEnumerable<IModulePackageFolder> GetModulePackageFolders(string applicationId)
        {
            var results = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId)
                .Where(x => x.SpecializationType == ModulePackageFolder.SpecializationType)
                .Select(x => new ModulePackageFolder(x)).ToList();
            return results;
        }

        public IEnumerable<IElementSettings> GetElementSettingses(string applicationId)
        {
            var results = _metadataManager.GetMetadata<IElement>("Module Builder", applicationId)
                .Where(x => x.SpecializationType == ElementSetting.RequiredSpecializationType)
                .Select(x => new ElementSetting(x)).ToList();
            return results;
        }

        public IEnumerable<IDecoratorDefinition> GetDecorators(IApplication application)
        {
            var templates = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id).Where(x => x.IsDecorator()).ToList();
            var result = templates.Select(x => new DecoratorDefinition(x)).ToList();
            return result;
        }

        public IEnumerable<IModelerReference> GetModelers(IElementApplication application)
        {
            var modelerElements = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id).Where(x => x.IsModeler()).ToList();
            var result = modelerElements.Select(x => new ModelerReference(x)).ToList();
            return result;
        }
    }
}