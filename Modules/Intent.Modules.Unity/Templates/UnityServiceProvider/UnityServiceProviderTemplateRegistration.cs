using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    [Description(UnityServiceProviderTemplate.Identifier)]
    public class IdentityGeneratorTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => UnityServiceProviderTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new UnityServiceProviderTemplate(project);
        }
    }
}
