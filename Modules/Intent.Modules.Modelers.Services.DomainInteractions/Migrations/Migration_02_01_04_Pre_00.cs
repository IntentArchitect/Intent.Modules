using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Migrations
{
    public class Migration_02_01_04_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_02_01_04_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.Services.DomainInteractions";
        [IntentFully]
        public string ModuleVersion => "2.1.4-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad");
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad",
                    Module = "Intent.Modelers.Services.DomainInteractions",
                    Title = "Domain Interaction Settings"
                };

                app.ModuleSettingGroups.Add(group);
            }

            var entityConvention = group.Settings.FirstOrDefault(s => s.Title == "4a1bc3ad-9ec2-499e-99d8-beba03a1d5bb");

            if (entityConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "4a1bc3ad-9ec2-499e-99d8-beba03a1d5bb",
                    Title = "Null Child Update Implementation",
                    Module = "Intent.Modelers.Services.DomainInteractions",
                    Hint = " How NULL child entities should be handled when updating a domain entity",
                    ControlType = ModuleSettingControlType.Select,
                    Value = "ignore",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "Ignore", Value = "ignore" },
                        new SettingFieldOptionPersistable { Description = "Set to NULL", Value = "set-to-null" }
                    ]
                });
            }

            app.SaveAllChanges();
        }

        public void Down()
        {
        }
    }
}