using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.Keys.Templates.IdentityGenerator
{
    [Description(IdentityGeneratorTemplate.Identifier)]
    public class IdentityGeneratorTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => IdentityGeneratorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new IdentityGeneratorTemplate(project);
        }
    }
}
