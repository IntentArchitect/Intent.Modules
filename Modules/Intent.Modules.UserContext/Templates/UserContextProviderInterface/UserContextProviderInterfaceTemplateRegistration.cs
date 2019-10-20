using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProviderInterface
{
    [Description(UserContextProviderInterfaceTemplate.Identifier)]
    public class UserContextProviderInterfaceTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => UserContextProviderInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new UserContextProviderInterfaceTemplate(project);
        }
    }
}
