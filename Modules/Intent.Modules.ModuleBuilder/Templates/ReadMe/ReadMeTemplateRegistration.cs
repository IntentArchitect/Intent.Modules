using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    public class ReadMeTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => ReadMeTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new ReadMeTemplate(project);
        }
    }
}