using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Common.Migrations
{
    public class Migration_03_11_00_Pre_01 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_03_11_00_Pre_01(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common";
        [IntentFully]
        public string ModuleVersion => "3.11.0-pre.1";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();

            AiOutputAnchorsHelper.EnsureExist(application);
        }


        public void Down()
        {
        }
    }
}