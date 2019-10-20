using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Migrations.Templates.ReadMe;
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public class MigrationReadMeTemplateRegistrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => MigrationReadMeTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new MigrationReadMeTemplate(project);
        }
    }
}
