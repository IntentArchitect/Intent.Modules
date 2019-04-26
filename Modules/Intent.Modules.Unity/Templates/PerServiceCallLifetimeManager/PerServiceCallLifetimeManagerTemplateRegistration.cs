using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


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

