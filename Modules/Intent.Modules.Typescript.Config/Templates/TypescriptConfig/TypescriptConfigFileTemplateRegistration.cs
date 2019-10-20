using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptConfig
{
    [Description(TypescriptConfigFileTemplate.Identifier)]
    public class TypescriptConfigFileTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => TypescriptConfigFileTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new TypescriptConfigFileTemplate(project);
        }
    }
}
