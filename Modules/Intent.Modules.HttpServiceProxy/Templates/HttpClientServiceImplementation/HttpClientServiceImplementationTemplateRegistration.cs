using Intent.Engine;
using Intent.Templates;

using System.ComponentModel;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation
{
    [Description(HttpClientServiceImplementationTemplate.IDENTIFIER)]
    public class HttpClientServiceImplementationTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpClientServiceImplementationTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpClientServiceImplementationTemplate(project);
        }
    }
}

