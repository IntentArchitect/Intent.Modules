using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    public class ReadMeTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => ReadMeTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget context)
        {
            return new ReadMeTemplate(context);
        }
    }
}