using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Migrations
{
    public class Migration_01_00_01_Pre_01 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_01_00_01_Pre_01(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.ModuleBuilder.AI.Skills";
        [IntentFully]
        public string ModuleVersion => "1.0.1-pre.1";

        public void Up()
        {
            ObsoleteSkillFolderCleanup.Run(_persistenceLoader);
        }

        public void Down()
        {
        }
    }
}
