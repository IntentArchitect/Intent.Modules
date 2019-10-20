using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public class DbMigrationsConfigurationTemplateRegistrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => DbMigrationsConfigurationTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new DbMigrationsConfigurationTemplate(project);
        }
    }
}
