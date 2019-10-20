using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference
{
    [Description(TypescriptDefinitelyTypedReferencesTemplate.Identifier)]
    public class TypescriptDefinitelyTypedReferencesTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => TypescriptDefinitelyTypedReferencesTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new TypescriptDefinitelyTypedReferencesTemplate(project);
        }
    }
}
