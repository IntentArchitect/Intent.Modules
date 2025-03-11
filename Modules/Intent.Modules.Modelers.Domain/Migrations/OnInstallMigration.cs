using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modelers.Domain.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        public OnInstallMigration()
        {
        }

        public string ModuleId { get; } = "Intent.Modelers.Domain";

        public void OnInstall()
        {

        }
    }
}