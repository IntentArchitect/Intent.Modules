using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static CSharpStyleConfiguration GetCSharpStyleConfiguration(this IApplicationSettingsProvider settings)
        {
            return new CSharpStyleConfiguration(settings.GetGroup("6cc9ca71-7670-48eb-b413-e47e72307cbd"));
        }
    }

    public class CSharpStyleConfiguration : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public CSharpStyleConfiguration(IGroupSettings groupSettings)
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
        public ConstructorInitializerOptions ConstructorInitializer() => new ConstructorInitializerOptions(_groupSettings.GetSetting("0c43b221-fec0-4df7-96d2-e7281ddf9207")?.Value);

        public class ConstructorInitializerOptions
        {
            public readonly string Value;

            public ConstructorInitializerOptions(string value)
            {
                Value = value;
            }

            public ConstructorInitializerOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "SameLine" => ConstructorInitializerOptionsEnum.SameLine,
                    "NewLine" => ConstructorInitializerOptionsEnum.NewLine,
                    "Mixed" => ConstructorInitializerOptionsEnum.Mixed,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSameLine()
            {
                return Value == "SameLine";
            }

            public bool IsNewLine()
            {
                return Value == "NewLine";
            }

            public bool IsMixed()
            {
                return Value == "Mixed";
            }
        }
        public enum ConstructorInitializerOptionsEnum
        {
            SameLine,
            NewLine,
            Mixed,
        }
    }
}