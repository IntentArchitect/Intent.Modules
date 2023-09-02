using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Metadata.RDBMS.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DatabaseSettings GetDatabaseSettings(this IApplicationSettingsProvider settings)
        {
            return new DatabaseSettings(settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9"));
        }
    }

    public class DatabaseSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DatabaseSettings(IGroupSettings groupSettings)
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

        public KeyTypeOptions KeyType() => new KeyTypeOptions(_groupSettings.GetSetting("ef83f85d-bb8d-4b10-8842-9f35f9f54165")?.Value);

        public class KeyTypeOptions
        {
            public readonly string Value;

            public KeyTypeOptions(string value)
            {
                Value = value;
            }

            public KeyTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "guid" => KeyTypeOptionsEnum.Guid,
                    "long" => KeyTypeOptionsEnum.Long,
                    "int" => KeyTypeOptionsEnum.Int,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsGuid()
            {
                return Value == "guid";
            }

            public bool IsLong()
            {
                return Value == "long";
            }

            public bool IsInt()
            {
                return Value == "int";
            }
        }

        public enum KeyTypeOptionsEnum
        {
            Guid,
            Long,
            Int,
        }

        public KeyCreationModeOptions KeyCreationMode() => new KeyCreationModeOptions(_groupSettings.GetSetting("5aca6e0c-1b64-425b-9046-f0bc81c44311")?.Value);

        public class KeyCreationModeOptions
        {
            public readonly string Value;

            public KeyCreationModeOptions(string value)
            {
                Value = value;
            }

            public KeyCreationModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "manual" => KeyCreationModeOptionsEnum.Manual,
                    "explicit" => KeyCreationModeOptionsEnum.Explicit,
                    "implicit" => KeyCreationModeOptionsEnum.Implicit,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsManual()
            {
                return Value == "manual";
            }

            public bool IsImplicit()
            {
                return Value == "implicit";
            }

            public bool IsExplicit()
            {
                return Value == "explicit";
            }
        }

        public enum KeyCreationModeOptionsEnum
        {
            Manual,
            Implicit,
            Explicit,
        }

        public string DecimalPrecisionAndScale() => _groupSettings.GetSetting("684582e0-125a-4ddd-a950-714e2af41f15")?.Value;
    }
}