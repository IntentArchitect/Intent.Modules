using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference
{
    [Description(TypescriptDefinitelyTypedReferencesTemplate.Identifier)]
    public class TypescriptDefinitelyTypedReferencesTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => TypescriptDefinitelyTypedReferencesTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new TypescriptDefinitelyTypedReferencesTemplate(project);
        }
    }
}
