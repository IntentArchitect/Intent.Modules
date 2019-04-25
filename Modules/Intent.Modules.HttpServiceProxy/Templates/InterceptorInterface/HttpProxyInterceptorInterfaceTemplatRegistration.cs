using Intent.SoftwareFactory.Engine;
using Intent.Templates

using System.ComponentModel;
using Intent.Modules.Common.Registrations;

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

