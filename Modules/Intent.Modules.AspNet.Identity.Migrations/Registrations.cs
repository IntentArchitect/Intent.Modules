using Intent.Modules.AspNet.Identity.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Modules.AspNet.Identity.Migrations.Templates.ReadMe;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.Identity.Migrations
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(IdentityDbMigrationsConfigurationTemplate.Identifier, project => new IdentityDbMigrationsConfigurationTemplate(project));
            RegisterTemplate(MigrationReadMeTemplate.Identifier, project => new MigrationReadMeTemplate(project));
        }
    }
}
