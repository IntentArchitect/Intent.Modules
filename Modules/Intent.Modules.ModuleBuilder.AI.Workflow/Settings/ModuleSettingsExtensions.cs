using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AIWorkflowSettings GetAIWorkflowSettings(this IApplicationSettingsProvider settings)
        {
            return new AIWorkflowSettings(settings.GetGroup("3467f2ef-cc10-476d-bc26-4948976b4083"));
        }
    }

    public class AIWorkflowSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AIWorkflowSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }

        public bool UsePreReleaseVersions() => bool.TryParse(_groupSettings.GetSetting("8b2a8402-0d7b-4273-96ae-4d857fcc16fb")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainModuleREADME() => bool.TryParse(_groupSettings.GetSetting("a1bd5e9f-5d24-49fe-91cd-8e823ed42535")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainModuleIcon() => bool.TryParse(_groupSettings.GetSetting("a4bdd803-c925-4711-aa99-f0fb218e4c7d")?.Value.ToPascalCase(), out var result) && result;
    }
}