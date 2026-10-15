using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class TemplateRegistrationRequiredEvent
    {
        public TemplateRegistrationRequiredEvent(string modelId, string templateId, string templateType, string role, string location)
            : this(modelId, templateId, templateType, role, location, null, null)
        {
        }

        public TemplateRegistrationRequiredEvent(string modelId, string templateId, string templateType, string role, string location, string reviewImportance, string classification)
        {
            ModelId = modelId;
            TemplateId = templateId;
            TemplateType = templateType;
            Role = role;
            Location = location;
            ReviewImportance = reviewImportance;
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
            ReviewImportance = template.GetReviewImportance();
            Classification = template.GetClassification();
        }

        public string SourceTemplateId { get; }
        public string ModelId { get; }
        public string ModelType { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string Role { get; }
        public string Location { get; }
        public string ReviewImportance { get; }

        [System.Obsolete("Use ReviewImportance.")]
        public string Severity => ReviewImportance;
        public string Classification { get; }
    }
}
