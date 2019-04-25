using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates


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
