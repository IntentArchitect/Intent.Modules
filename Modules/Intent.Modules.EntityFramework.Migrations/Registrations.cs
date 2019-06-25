using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Modules.EntityFramework.Migrations.Templates.ReadMe;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.EntityFramework.Migrations
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metadataManager)
        {
            RegisterTemplate(DbMigrationsConfigurationTemplate.Identifier, project => new DbMigrationsConfigurationTemplate(project));
            RegisterTemplate(MigrationReadMeTemplate.Identifier, project => new MigrationReadMeTemplate(project));
        }
    }
}
