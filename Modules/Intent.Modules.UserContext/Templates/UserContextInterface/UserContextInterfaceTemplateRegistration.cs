using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextInterface
{
    [Description(UserContextInterfaceTemplate.Identifier)]
    public class UserContextInterfaceTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => UserContextInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new UserContextInterfaceTemplate(project);
        }
    }
}
