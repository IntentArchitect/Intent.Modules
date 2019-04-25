using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface
{
    [Description(HttpClientServiceInterfaceTemplate.IDENTIFIER)]
    public class HttpClientServiceInterfaceTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpClientServiceInterfaceTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpClientServiceInterfaceTemplate(project);
        }
    }
}

