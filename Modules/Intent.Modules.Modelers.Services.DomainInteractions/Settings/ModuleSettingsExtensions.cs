using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DomainInteractionSettings GetDomainInteractionSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainInteractionSettings(settings.GetGroup("0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad"));
        }
    }

    public class DomainInteractionSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DomainInteractionSettings(IGroupSettings groupSettings)
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
        public DefaultMappingModeOptions DefaultMappingMode() => new DefaultMappingModeOptions(_groupSettings.GetSetting("14bc1e98-930b-494c-97a9-5da4e1f3a5fa")?.Value);

        public class DefaultMappingModeOptions
        {
            public readonly string Value;

            public DefaultMappingModeOptions(string value)
            {
                Value = value;
            }

            public DefaultMappingModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "basic" => DefaultMappingModeOptionsEnum.Basic,
                    "advanced" => DefaultMappingModeOptionsEnum.Advanced,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsBasic()
            {
                return Value == "basic";
            }

            public bool IsAdvanced()
            {
                return Value == "advanced";
            }
        }

        public enum DefaultMappingModeOptionsEnum
        {
            Basic,
            Advanced,
        }

        public DefaultQueryImplementationOptions DefaultQueryImplementation() => new DefaultQueryImplementationOptions(_groupSettings.GetSetting("61ced3b4-e1d8-4274-b84d-d9b8e0c3143f")?.Value);

        public class DefaultQueryImplementationOptions
        {
            public readonly string Value;

            public DefaultQueryImplementationOptions(string value)
            {
                Value = value;
            }

            public DefaultQueryImplementationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "default" => DefaultQueryImplementationOptionsEnum.Default,
                    "project-to" => DefaultQueryImplementationOptionsEnum.ProjectTo,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsDefault()
            {
                return Value == "default";
            }

            public bool IsProjectTo()
            {
                return Value == "project-to";
            }
        }

        public enum DefaultQueryImplementationOptionsEnum
        {
            Default,
            ProjectTo,
        }
    }
}