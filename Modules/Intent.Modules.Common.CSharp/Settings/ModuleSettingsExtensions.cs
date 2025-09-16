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
                    "same-line" => ConstructorInitializerOptionsEnum.SameLine,
                    "new-line" => ConstructorInitializerOptionsEnum.NewLine,
                    "depends-on-length" => ConstructorInitializerOptionsEnum.DependsOnLength,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSameLine()
            {
                return Value == "same-line";
            }

            public bool IsNewLine()
            {
                return Value == "new-line";
            }

            public bool IsDependsOnLength()
            {
                return Value == "depends-on-length";
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
                    "same-line" => ParameterPlacementOptionsEnum.SameLine,
                    "depends-on-length" => ParameterPlacementOptionsEnum.DependsOnLength,
                    "default" => ParameterPlacementOptionsEnum.Default,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSameLine()
            {
                return Value == "same-line";
            }

            public bool IsDependsOnLength()
            {
                return Value == "depends-on-length";
            }

            public bool IsDefault()
            {
                return Value == "default";
            }
        }

        public enum ParameterPlacementOptionsEnum
        {
            SameLine,
            DependsOnLength,
            Default,
        }

        public BlankLinesBetweenMembersOptions BlankLinesBetweenMembers() => new BlankLinesBetweenMembersOptions(_groupSettings.GetSetting("727bb639-f91c-4f86-bd81-b1eccd7e4d45")?.Value);

        public class BlankLinesBetweenMembersOptions
        {
            public readonly string Value;

            public BlankLinesBetweenMembersOptions(string value)
            {
                Value = value;
            }

            public BlankLinesBetweenMembersOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "default" => BlankLinesBetweenMembersOptionsEnum.Default,
                    "blank-line" => BlankLinesBetweenMembersOptionsEnum.BlankLine,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsDefault()
            {
                return Value == "default";
            }

            public bool IsBlankLine()
            {
                return Value == "blank-line";
            }
        }

        public enum BlankLinesBetweenMembersOptionsEnum
        {
            Default,
            BlankLine,
        }
    }
}