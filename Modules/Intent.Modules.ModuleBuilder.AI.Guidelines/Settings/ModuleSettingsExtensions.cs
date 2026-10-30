using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Guidelines.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AIGuidelinesSettings GetAIGuidelinesSettings(this IApplicationSettingsProvider settings)
        {
            return new AIGuidelinesSettings(settings.GetGroup("a7267a53-6d79-4838-8fbc-82f3b6ef960a"));
        }
    }

    public class AIGuidelinesSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AIGuidelinesSettings(IGroupSettings groupSettings)
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

        public bool UsePreReleaseVersions() => bool.TryParse(_groupSettings.GetSetting("0e96cf15-32bf-4e34-a082-a79f84c5668e")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainModuleREADME() => bool.TryParse(_groupSettings.GetSetting("b6cf3b08-dc4d-49b3-a11e-4b41c10ecb97")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainModuleIcon() => bool.TryParse(_groupSettings.GetSetting("f832145b-74f3-48bf-878e-16b6f0203781")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainModuleContext() => bool.TryParse(_groupSettings.GetSetting("f82fe4f9-a5fb-444e-8248-fdcdef66c920")?.Value.ToPascalCase(), out var result) && result;

        public bool MaintainReleaseNotes() => bool.TryParse(_groupSettings.GetSetting("af5fa90e-3199-4f3e-80af-a374527fe48d")?.Value.ToPascalCase(), out var result) && result;

        public bool InstallAgentGateHooks() => bool.TryParse(_groupSettings.GetSetting("c4066c25-1ee9-4b0c-a3ed-f84148ea0512")?.Value.ToPascalCase(), out var result) && result;
    }
}