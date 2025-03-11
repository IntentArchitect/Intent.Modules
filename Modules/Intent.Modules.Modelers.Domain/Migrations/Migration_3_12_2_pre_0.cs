using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using System.Diagnostics;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modelers.Domain.Migrations
{
    public class Migration_3_12_2_pre_0 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_3_12_2_pre_0(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId { get; } = "Intent.Modelers.Domain";
        public string ModuleVersion { get; } = "3.12.2-pre.0";

        public void Up()
        {
            Debugger.Launch();

            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Title == "Domain Settings");
            if (group == null) 
            {
                return;
            }

            var setting = group.Settings.FirstOrDefault(s => s.Title == "Entity Naming Convention");

            if (setting == null)
            {
                return;
            }

            setting.Value = "pascal-case";

            app.SaveAllChanges();
        }

        public void Down()
        {
        }
    }
}