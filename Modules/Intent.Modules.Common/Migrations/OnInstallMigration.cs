using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Common.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common";

        public void OnInstall()
        {
            var application = _persistenceLoader.LoadCurrentApplication();

            AiOutputAnchorsHelper.EnsureExist(application);

        }
    }
}