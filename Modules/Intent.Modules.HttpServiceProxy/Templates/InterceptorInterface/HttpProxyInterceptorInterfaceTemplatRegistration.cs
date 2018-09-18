using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface
{
    [Description(HttpProxyInterceptorInterfaceTemplate.IDENTIFIER)]
    public class HttpProxyInterceptorInterfaceTemplatRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpProxyInterceptorInterfaceTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpProxyInterceptorInterfaceTemplate(project);
        }
    }
}

