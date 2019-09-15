using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class TemplateDefinition : ModuleBuilderElementBase, ITemplateDefinition
    {
        private readonly ModuleBuilderMetadataProvider _metadataProvider;

        public TemplateDefinition(IElement element, ModuleBuilderMetadataProvider metadataProvider) : base(element)
        {
            _metadataProvider = metadataProvider;
        }

        public string GetModelerName()
        {
            return GetModeler().Name;
        }

        public string GetModelTypeName()
        {
            return GetModelType().Name;
        }

        public IModelerModelType GetModelType()
        {
            var modelTypeId = this.GetStereotypeProperty(ModelExtensions.TemplateSettingsStereotype, "Model Type", string.Empty);
            return GetModeler()?.ModelTypes.SingleOrDefault(x => x.Id == modelTypeId);
        }

        public IModeler GetModeler()
        {
            var modelerId = this.GetStereotypeProperty(ModelExtensions.TemplateSettingsStereotype, "Modeler", string.Empty);
            return _metadataProvider.GetModelers(Application).SingleOrDefault(x => x.Id == modelerId);
        }

        public IEnumerable<ITemplateDependencyDefinition> GetTemplateDependencies()
        {
            return _element.Attributes.Where(x => true).Select(x => new TemplateDependencyDefinition(x.Type.Element)).ToList();
        }
    }
}