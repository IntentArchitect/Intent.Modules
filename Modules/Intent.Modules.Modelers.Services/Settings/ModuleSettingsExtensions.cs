using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ServiceSettings GetServiceSettings(this IApplicationSettingsProvider settings)
        {
            return new ServiceSettings(settings.GetGroup("370723b8-0896-465f-acc7-099d8f36e052"));
        }
    }

    public class ServiceSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ServiceSettings(IGroupSettings groupSettings)
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
        public PropertyNamingConventionOptions PropertyNamingConvention() => new PropertyNamingConventionOptions(_groupSettings.GetSetting("8df5e8ae-1a56-4507-87e9-4f73cd69ba50")?.Value);

        public class PropertyNamingConventionOptions
        {
            public readonly string Value;

            public PropertyNamingConventionOptions(string value)
            {
                Value = value;
            }

            public PropertyNamingConventionOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "manual" => PropertyNamingConventionOptionsEnum.Manual,
                    "pascal-case" => PropertyNamingConventionOptionsEnum.PascalCase,
                    "camel-case" => PropertyNamingConventionOptionsEnum.CamelCase,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsManual()
            {
                return Value == "manual";
            }

            public bool IsPascalCase()
            {
                return Value == "pascal-case";
            }

            public bool IsCamelCase()
            {
                return Value == "camel-case";
            }
        }

        public enum PropertyNamingConventionOptionsEnum
        {
            Manual,
            PascalCase,
            CamelCase,
        }
        public EntityNamingConventionOptions EntityNamingConvention() => new EntityNamingConventionOptions(_groupSettings.GetSetting("625c6211-0dc7-4190-af49-6eadb82c7015")?.Value);

        public class EntityNamingConventionOptions
        {
            public readonly string Value;

            public EntityNamingConventionOptions(string value)
            {
                Value = value;
            }

            public EntityNamingConventionOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "pascal-case" => EntityNamingConventionOptionsEnum.PascalCase,
                    "camel-case" => EntityNamingConventionOptionsEnum.CamelCase,
                    "manual" => EntityNamingConventionOptionsEnum.Manual,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsPascalCase()
            {
                return Value == "pascal-case";
            }

            public bool IsCamelCase()
            {
                return Value == "camel-case";
            }

            public bool IsManual()
            {
                return Value == "manual";
            }
        }

        public enum EntityNamingConventionOptionsEnum
        {
            PascalCase,
            CamelCase,
            Manual,
        }
        public OperationNamingConventionOptions OperationNamingConvention() => new OperationNamingConventionOptions(_groupSettings.GetSetting("9add7769-0034-4c7f-987e-bb592cfd335e")?.Value);

        public class OperationNamingConventionOptions
        {
            public readonly string Value;

            public OperationNamingConventionOptions(string value)
            {
                Value = value;
            }

            public OperationNamingConventionOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "camel-case" => OperationNamingConventionOptionsEnum.CamelCase,
                    "manual" => OperationNamingConventionOptionsEnum.Manual,
                    "pascal-case" => OperationNamingConventionOptionsEnum.PascalCase,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsCamelCase()
            {
                return Value == "camel-case";
            }

            public bool IsManual()
            {
                return Value == "manual";
            }

            public bool IsPascalCase()
            {
                return Value == "pascal-case";
            }
        }

        public enum OperationNamingConventionOptionsEnum
        {
            CamelCase,
            Manual,
            PascalCase,
        }
    }
}