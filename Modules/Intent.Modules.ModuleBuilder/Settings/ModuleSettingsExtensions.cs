using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ModuleBuilderSettings GetModuleBuilderSettings(this IApplicationSettingsProvider settings)
        {
            return new ModuleBuilderSettings(settings.GetGroup("b2c4252b-cfae-43c5-9682-803aa0b84c87"));
        }
    }

    public class ModuleBuilderSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ModuleBuilderSettings(IGroupSettings groupSettings)
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

        public bool CreatePartialAPIModels() => bool.TryParse(_groupSettings.GetSetting("b06c3926-23e5-49dd-a59d-93ef16b9777e")?.Value.ToPascalCase(), out var result) && result;
    }
}