using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    [Description(UnityConfigTemplate.Identifier)]
    public class UnityConfigTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => UnityConfigTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new UnityConfigTemplate(project, project.Application.EventDispatcher);
        }
    }
}
