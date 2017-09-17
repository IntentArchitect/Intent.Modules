using Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Modules.EntityFramework.Migrations.Templates.ReadMe;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFramework.Migrations
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(DbMigrationsConfigurationTemplate.Identifier, project => new DbMigrationsConfigurationTemplate(project));
            RegisterTemplate(MigrationReadMeTemplate.Identifier, project => new MigrationReadMeTemplate(project));
        }
    }
}
