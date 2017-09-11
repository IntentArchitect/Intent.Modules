using System.Linq;
using Intent.Packages.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Packages.EntityFramework.Migrations.Templates.ReadMe;
using Intent.Packages.EntityFramework.Templates.DbContext;
using Intent.Packages.EntityFramework.Templates.DeleteVisitor;
using Intent.Packages.EntityFramework.Templates.EFMapping;
using Intent.Packages.EntityFramework.Templates.Repository;
using Intent.Packages.EntityFramework.Templates.RepositoryContract;
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
            RegisterTemplate(DbMigrationsConfigurationTemplate.Identifier, project => new DbMigrationsConfigurationTemplate(project));
            RegisterTemplate(MigrationReadMeTemplate.Identifier, project => new MigrationReadMeTemplate(project));
        }
    }
}
