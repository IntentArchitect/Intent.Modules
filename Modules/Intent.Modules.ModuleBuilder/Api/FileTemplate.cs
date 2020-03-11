using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class FileTemplate : ModuleBuilderElementBase, IFileTemplate
    {

        public FileTemplate(IElement element) : base(element)
        {
        }

        public string GetModelerName()
        {
            return GetModeler().Name;
        }

        public string FileExtension => _element.GetStereotypeProperty(ModelExtensions.FileTemplateSettingsStereotype, "File Extension", "unknown");

        public string GetModelTypeName()
        {
            return GetModelType().InterfaceName;
        }

        public IModelerModelType GetModelType()
        {
            var modelTypeId = this.GetStereotypeProperty(ModelExtensions.FileTemplateSettingsStereotype, "Model Type", string.Empty);
            return GetModeler()?.ModelTypes.SingleOrDefault(x => x.Id == modelTypeId);
        }

        public IModelerReference GetModeler()
        {
            var element = this.GetStereotypeProperty<IElement>(ModelExtensions.FileTemplateSettingsStereotype, "Modeler");
            return element == null ? null : new ModelerReference(element);
        }

        public IEnumerable<ITemplateDependencyDefinition> GetTemplateDependencies()
        {
            return _element.Attributes.Where(x => true).Select(x => new TemplateDependencyDefinition(x.Type.Element)).ToList();
        }

        public override string ToString()
        {
            return $"[{nameof(FileTemplate)}: {Name}]";
        }
    }
}