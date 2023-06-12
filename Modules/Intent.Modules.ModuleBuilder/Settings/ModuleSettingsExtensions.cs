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
        public DependencyVersionOverwriteBehaviorOptions DependencyVersionOverwriteBehavior() => new DependencyVersionOverwriteBehaviorOptions(_groupSettings.GetSetting("9c8e6982-e036-4d35-bab1-9bb02382d7c3")?.Value);

        public class DependencyVersionOverwriteBehaviorOptions
        {
            public readonly string Value;

            public DependencyVersionOverwriteBehaviorOptions(string value)
            {
                Value = value;
            }

            public DependencyVersionOverwriteBehaviorOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "always" => DependencyVersionOverwriteBehaviorOptionsEnum.Always,
                    "if-newer" => DependencyVersionOverwriteBehaviorOptionsEnum.IfNewer,
                    "never" => DependencyVersionOverwriteBehaviorOptionsEnum.Never,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAlways()
            {
                return Value == "always";
            }

            public bool IsIfNewer()
            {
                return Value == "if-newer";
            }

            public bool IsNever()
            {
                return Value == "never";
            }
        }

        public enum DependencyVersionOverwriteBehaviorOptionsEnum
        {
            Always,
            IfNewer,
            Never,
        }
    }
}