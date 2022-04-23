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
    }
}