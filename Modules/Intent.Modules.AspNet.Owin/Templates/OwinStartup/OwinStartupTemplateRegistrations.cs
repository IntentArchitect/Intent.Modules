using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public class OwinStartupTemplateRegistrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => OwinStartupTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new OwinStartupTemplate(project);
        }
    }
}
