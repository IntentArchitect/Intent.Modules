using System.ComponentModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager
{
    [Description(PerServiceCallLifetimeManagerTemplate.Identifier)]
    public class PerServiceCallLifetimeManagerTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => PerServiceCallLifetimeManagerTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new PerServiceCallLifetimeManagerTemplate(project);
        }
    }
}

