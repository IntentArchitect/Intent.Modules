using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextStatic
{
    [Description(UserContextStaticTemplate.Identifier)]
    public class UserContextStaticTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => UserContextStaticTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new UserContextStaticTemplate(project);
        }
    }
}
