using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class TemplateRegistrationRequiredEvent
    {
        public TemplateRegistrationRequiredEvent(string modelId, string templateId, string templateType, string role, string location, string severity, string classification)
        {
            ModelId = modelId;
            TemplateId = templateId;
            TemplateType = templateType;
            Role = role;
            Location = location;
            Severity = severity;
            Classification = classification;
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
            Severity = template.GetSeverity();
            Classification = template.GetClassification();
        }

        public string SourceTemplateId { get; }
        public string ModelId { get; }
        public string ModelType { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string Role { get; }
        public string Location { get; }
        public string Severity { get; }
        public string Classification { get; }
    }
}
