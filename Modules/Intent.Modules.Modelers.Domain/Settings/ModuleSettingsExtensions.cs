using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Namespaces)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DomainSettings GetDomainSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainSettings(settings.GetGroup("c4d1e35c-7c0d-4926-afe0-18f17563ce17"));
        }
    }

    public class DomainSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DomainSettings(IGroupSettings groupSettings)
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
        public AttributeNamingConventionOptions AttributeNamingConvention() => new AttributeNamingConventionOptions(_groupSettings.GetSetting("c3789138-6649-47e4-901b-5f6469583cb7")?.Value);

        public class AttributeNamingConventionOptions
        {
            public readonly string Value;

            public AttributeNamingConventionOptions(string value)
            {
                Value = value;
            }

            public AttributeNamingConventionOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "manual" => AttributeNamingConventionOptionsEnum.Manual,
                    "pascal-case" => AttributeNamingConventionOptionsEnum.PascalCase,
                    "camel-case" => AttributeNamingConventionOptionsEnum.CamelCase,
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

        public enum AttributeNamingConventionOptionsEnum
        {
            Manual,
            PascalCase,
            CamelCase,
        }

        public EntityNamingConventionOptions EntityNamingConvention() => new EntityNamingConventionOptions(_groupSettings.GetSetting("dcb5114c-39d0-4880-b6c6-312bbb3ceac1")?.Value);

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
                    "manual" => EntityNamingConventionOptionsEnum.Manual,
                    "pascal-case" => EntityNamingConventionOptionsEnum.PascalCase,
                    "camel-case" => EntityNamingConventionOptionsEnum.CamelCase,
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

        public enum EntityNamingConventionOptionsEnum
        {
            Manual,
            PascalCase,
            CamelCase,
        }

        public OperationNamingConventionOptions OperationNamingConvention() => new OperationNamingConventionOptions(_groupSettings.GetSetting("128d9880-cbf7-469f-a5af-915bc3d71874")?.Value);

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
                    "manual" => OperationNamingConventionOptionsEnum.Manual,
                    "pascal-case" => OperationNamingConventionOptionsEnum.PascalCase,
                    "camel-case" => OperationNamingConventionOptionsEnum.CamelCase,
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

        public enum OperationNamingConventionOptionsEnum
        {
            Manual,
            PascalCase,
            CamelCase,
        }
    }
}