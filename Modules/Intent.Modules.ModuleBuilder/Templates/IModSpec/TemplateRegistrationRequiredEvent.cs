using System;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.SdkEvolutionHelpers;

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

        [FixFor_Version4("Remove need to check for TemplateRegistrationModel by having everything else use the interface method instead.")]
        public TemplateRegistrationRequiredEvent(IModuleBuilderTemplate template)
        {
            SourceTemplateId = template.Id;
            ModelId = ((IMetadataModel) template.Model).Id;
            ModelType = template.GetModelType();
            TemplateId = template.GetTemplateId();
            TemplateType = template.TemplateType();
            Role = template.GetRole();
            Location = template switch
            {
                IModuleBuilderTemplateWithDefaultLocation templateWithDefaultLocation => templateWithDefaultLocation.GetDefaultLocation(),
                _ when template.Model is TemplateRegistrationModel model => model.GetLocation(),
                _ => throw new ArgumentOutOfRangeException()
            };
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