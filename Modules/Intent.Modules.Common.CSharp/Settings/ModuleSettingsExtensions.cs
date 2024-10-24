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
                    "DependsOnLength" => ConstructorInitializerOptionsEnum.DependsOnLength,
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

            public bool IsDependsOnLength()
            {
                return Value == "DependsOnLength";
            }
        }
        public enum ConstructorInitializerOptionsEnum
        {
            SameLine,
            NewLine,
            DependsOnLength,
        }
        public ParameterPlacementOptions ParameterPlacement() => new ParameterPlacementOptions(_groupSettings.GetSetting("6483b559-92bb-43d7-a0b4-af20ded10a1b")?.Value);

        public class ParameterPlacementOptions
        {
            public readonly string Value;

            public ParameterPlacementOptions(string value)
            {
                Value = value;
            }

            public ParameterPlacementOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "SameLine" => ParameterPlacementOptionsEnum.SameLine,
                    "NewLine" => ParameterPlacementOptionsEnum.NewLine,
                    "DependsOnLength" => ParameterPlacementOptionsEnum.DependsOnLength,
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

            public bool IsDependsOnLength()
            {
                return Value == "DependsOnLength";
            }
        }

        public enum ParameterPlacementOptionsEnum
        {
            SameLine,
            NewLine,
            DependsOnLength,
        }
    }
}