using System.ComponentModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Templates.IdentityGenerator
{
    [Description(IdentityGeneratorTemplate.Identifier)]
    public class IdentityGeneratorTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => IdentityGeneratorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new IdentityGeneratorTemplate(project);
        }
    }
}
