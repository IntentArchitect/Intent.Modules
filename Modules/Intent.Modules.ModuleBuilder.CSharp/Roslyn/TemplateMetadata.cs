namespace Intent.Modules.Common.Templates
{
    public class TemplateMetadata
    {
        public string TemplateId { get; }
        public TemplateVersion Version { get; }

        public TemplateMetadata(string templateId, TemplateVersion version)
        {
            TemplateId = templateId;
            Version = version;
        }
    }
}
