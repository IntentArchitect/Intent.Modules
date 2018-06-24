using System.ComponentModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsConfiguration
{
    [Description(DbMigrationsConfigurationTemplate.Identifier)]
    public class DbMigrationsConfigurationTemplateRegistration : NoModelTemplateRegistrationBase
    {

        public override string TemplateId => DbMigrationsConfigurationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new DbMigrationsConfigurationTemplate(project);
        }
    }
}
