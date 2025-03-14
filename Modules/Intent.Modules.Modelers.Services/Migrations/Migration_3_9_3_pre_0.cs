using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.Migrations
{
    public class Migration_3_9_3_pre_0 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_3_9_3_pre_0(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId { get; } = "Intent.Modelers.Services";
        public string ModuleVersion { get; } = "3.9.3-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "370723b8-0896-465f-acc7-099d8f36e052");
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "370723b8-0896-465f-acc7-099d8f36e052",
                    Module = "Intent.Modelers.Domain",
                    Title = "Service Settings"
                };

                app.ModuleSettingGroups.Add(group);
            }

            var propertyConvention = group.Settings.FirstOrDefault(s => s.Title == "8df5e8ae-1a56-4507-87e9-4f73cd69ba50");

            if (propertyConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "8df5e8ae-1a56-4507-87e9-4f73cd69ba50",
                    Title = "Property Naming Convention",
                    Module = "Intent.Modelers.Services",
                    Hint = "Convention applied when creating and renaming properties.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = app.Modules.Any(m => m.ModuleId == "Intent.Common.CSharp") ? "pascal-case" : "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "Manual", Value = "manual" },
                        new SettingFieldOptionPersistable { Description = "Pascal Case", Value = "pascal-case" },
                        new SettingFieldOptionPersistable { Description = "Camel Case", Value = "camel-case" },
                    ]
                });
            }

            var entityConvention = group.Settings.FirstOrDefault(s => s.Title == "625c6211-0dc7-4190-af49-6eadb82c7015");

            if (entityConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "625c6211-0dc7-4190-af49-6eadb82c7015",
                    Title = "Entity Naming Convention",
                    Module = "Intent.Modelers.Services",
                    Hint = "Convention applied when creating and renaming entities.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = app.Modules.Any(m => m.ModuleId == "Intent.Common.CSharp") ? "pascal-case" : "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "Manual", Value = "manual" },
                        new SettingFieldOptionPersistable { Description = "Pascal Case", Value = "pascal-case" },
                        new SettingFieldOptionPersistable { Description = "Camel Case", Value = "camel-case" },
                    ]
                });
            }

            var operationConvention = group.Settings.FirstOrDefault(s => s.Title == "9add7769-0034-4c7f-987e-bb592cfd335e");

            if (operationConvention == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "9add7769-0034-4c7f-987e-bb592cfd335e",
                    Title = "Operation Naming Convention",
                    Module = "Intent.Modelers.Services",
                    Hint = "Convention applied when creating and renaming operation.",
                    ControlType = ModuleSettingControlType.Select,
                    Value = app.Modules.Any(m => m.ModuleId == "Intent.Common.CSharp") ? "pascal-case" : "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "Manual", Value = "manual" },
                        new SettingFieldOptionPersistable { Description = "Pascal Case", Value = "pascal-case" },
                        new SettingFieldOptionPersistable { Description = "Camel Case", Value = "camel-case" },
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