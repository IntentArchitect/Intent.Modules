using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    [Description(DbMigrationsReadMeTemplate.Identifier)]
    public class DbMigrationsReadMeTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => DbMigrationsReadMeTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new DbMigrationsReadMeTemplate(project);
        }
    }
}
