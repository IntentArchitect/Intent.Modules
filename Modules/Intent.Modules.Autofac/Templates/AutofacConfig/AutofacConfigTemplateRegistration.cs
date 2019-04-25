using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.Templates


namespace Intent.Modules.Autofac.Templates.AutofacConfig
{
    [Description(AutofacConfigTemplate.Identifier)]
    public class AutofacConfigTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => AutofacConfigTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new AutofacConfigTemplate(project, project.Application.EventDispatcher);
        }
    }
}
