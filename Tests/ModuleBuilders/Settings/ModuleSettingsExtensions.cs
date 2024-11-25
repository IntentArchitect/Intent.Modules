using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace ModuleBuilders.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static NewModuleSettingsGroup GetNewModuleSettingsGroup(this IApplicationSettingsProvider settings)
        {
            return new NewModuleSettingsGroup(settings.GetGroup("141601aa-92b2-4462-bf45-eda256a91148"));
        }
    }

    public class NewModuleSettingsGroup : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public NewModuleSettingsGroup(IGroupSettings groupSettings)
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
        public NewSettingsFieldOptions[] NewSettingsField() => JsonSerializer.Deserialize<string[]>(_groupSettings.GetSetting("a12ffcca-336e-4509-ac5c-f795c4a689b0")?.Value ?? "[]")?.Select(x => new NewSettingsFieldOptions(x)).ToArray();

        public class NewSettingsFieldOptions
        {
            public readonly string Value;

            public NewSettingsFieldOptions(string value)
            {
                Value = value;
            }

            public NewSettingsFieldOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "Settings Option" => NewSettingsFieldOptionsEnum.SettingsOption,
                    "Settings Option1" => NewSettingsFieldOptionsEnum.SettingsOption1,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSettingsOption()
            {
                return Value == "Settings Option";
            }

            public bool IsSettingsOption1()
            {
                return Value == "Settings Option1";
            }
        }

        public enum NewSettingsFieldOptionsEnum
        {
            SettingsOption,
            SettingsOption1,
        }
    }
}