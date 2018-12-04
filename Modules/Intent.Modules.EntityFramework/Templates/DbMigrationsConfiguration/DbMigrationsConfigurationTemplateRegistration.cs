using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration
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
