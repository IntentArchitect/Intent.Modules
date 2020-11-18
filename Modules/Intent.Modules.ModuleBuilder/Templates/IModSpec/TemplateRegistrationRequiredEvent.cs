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
        public string ModelId { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string Role { get; }
        public string Location { get; }
    }
}