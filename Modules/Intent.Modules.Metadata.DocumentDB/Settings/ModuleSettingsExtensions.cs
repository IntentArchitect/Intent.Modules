using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Metadata.DocumentDB.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DocumentDatabase GetDocumentDatabase(this IApplicationSettingsProvider settings)
        {
            return new DocumentDatabase(settings.GetGroup("d5581fe8-7385-4bb6-88dc-8940e20ec1d4"));
        }
    }

    public class DocumentDatabase : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DocumentDatabase(IGroupSettings groupSettings)
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
        public IdTypeOptions IdType() => new IdTypeOptions(_groupSettings.GetSetting("8bcf2b5c-cd35-47d6-8705-7e8e83494083")?.Value);

        public class IdTypeOptions
        {
            public readonly string Value;

            public IdTypeOptions(string value)
            {
                Value = value;
            }

            public IdTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "object-id" => IdTypeOptionsEnum.ObjectId,
                    "guid" => IdTypeOptionsEnum.Guid,
                    "int" => IdTypeOptionsEnum.Int,
                    "long" => IdTypeOptionsEnum.Long,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsObjectId()
            {
                return Value == "object-id";
            }

            public bool IsGuid()
            {
                return Value == "guid";
            }

            public bool IsInt()
            {
                return Value == "int";
            }

            public bool IsLong()
            {
                return Value == "long";
            }
        }

        public enum IdTypeOptionsEnum
        {
            ObjectId,
            Guid,
            Int,
            Long,
        }
    }
}