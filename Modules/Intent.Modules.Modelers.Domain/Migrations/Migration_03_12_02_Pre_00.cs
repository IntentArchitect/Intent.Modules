using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modelers.Domain.Migrations
{
    public class Migration_03_12_02_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_12_02_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId { get; } = "Intent.Modelers.Domain";
        public string ModuleVersion { get; } = "3.12.2-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "c4d1e35c-7c0d-4926-afe0-18f17563ce17");
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "c4d1e35c-7c0d-4926-afe0-18f17563ce17",
                    Module = "Intent.Modelers.Domain",
                    Title = "Domain Settings"
                };

                app.ModuleSettingGroups.Add(group);
            }

            var entityConvention = group.Settings.FirstOrDefault(s => s.Title == "dcb5114c-39d0-4880-b6c6-312bbb3ceac1");

            if (entityConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "dcb5114c-39d0-4880-b6c6-312bbb3ceac1",
                    Title = "Entity Naming Convention",
                    Module = "Intent.Modelers.Domain",
                    Hint = "Convention applied when creating and renaming entities.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = app.Modules.Any(m => m.ModuleId == "Intent.Common.CSharp") ? "pascal-case" : "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable{ Description = "Manual", Value = "manual"},
                        new SettingFieldOptionPersistable{ Description = "Pascal Case", Value = "pascal-case"},
                        new SettingFieldOptionPersistable{ Description = "Camel Case", Value = "camel-case"},
                    ]
                });
            }

            var operationConvention = group.Settings.FirstOrDefault(s => s.Title == "128d9880-cbf7-469f-a5af-915bc3d71874");

            if (operationConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "128d9880-cbf7-469f-a5af-915bc3d71874",
                    Title = "Operation Naming Convention",
                    Module = "Intent.Modelers.Domain",
                    Hint = "Convention applied when creating and renaming operation.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = app.Modules.Any(m => m.ModuleId == "Intent.Common.CSharp") ? "pascal-case" : "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable{ Description = "Manual", Value = "manual"},
                        new SettingFieldOptionPersistable{ Description = "Pascal Case", Value = "pascal-case"},
                        new SettingFieldOptionPersistable{ Description = "Camel Case", Value = "camel-case"},
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