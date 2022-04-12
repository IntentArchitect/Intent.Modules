using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class TemplateRegistrationRequiredEvent
    {
        public TemplateRegistrationRequiredEvent(string modelId, string templateId, string templateType, string role, string location)
        {
            ModelId = modelId;
            TemplateId = templateId;
            TemplateType = templateType;
            Role = role;
            Location = location;
        }

        public TemplateRegistrationRequiredEvent(IModuleBuilderTemplate template)
        {
            SourceTemplateId = template.Id;
            ModelId = ((IMetadataModel)template.Model).Id;
            ModelType = template.GetModelType();
            TemplateId = template.GetTemplateId();
            TemplateType = template.TemplateType();
            Role = template.GetRole();
            Location = template.GetDefaultLocation();
        }

        public string SourceTemplateId { get; }
        public string ModelId { get; }
        public string ModelType { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string Role { get; }
        public string Location { get; }
    }
}