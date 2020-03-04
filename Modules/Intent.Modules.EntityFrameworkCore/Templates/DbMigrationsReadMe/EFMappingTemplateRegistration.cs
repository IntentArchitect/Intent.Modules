using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

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
