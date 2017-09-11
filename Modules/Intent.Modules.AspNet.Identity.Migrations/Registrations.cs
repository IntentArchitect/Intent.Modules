using System.Linq;
using Intent.Packages.AspNet.Identity.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Packages.AspNet.Identity.Migrations.Templates.ReadMe;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.EntityFramework
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
