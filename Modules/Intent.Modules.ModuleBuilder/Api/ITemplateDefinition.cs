using System.Collections;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ITemplateDefinition : IModuleBuilderElement
    {
        IModelerModelType GetModelType();
        IModeler GetModeler();
        string GetModelTypeName();
        string GetModelerName();
    }

    public class TemplateDefinition : ModuleBuilderElementBase, ITemplateDefinition
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
    }
}
