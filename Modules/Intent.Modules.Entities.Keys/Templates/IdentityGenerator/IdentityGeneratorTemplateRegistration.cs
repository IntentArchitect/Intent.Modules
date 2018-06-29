using System.ComponentModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.Keys.Templates.IdentityGenerator
{
    [Description(Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate.Identifier)]
    public class IdentityGeneratorTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate(project);
        }
    }
}
