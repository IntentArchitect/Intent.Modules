using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface
{
    [Description(HttpClientServiceInterfaceTemplate.IDENTIFIER)]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpClientServiceInterfaceTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpClientServiceInterfaceTemplate(project);
        }
    }
}

