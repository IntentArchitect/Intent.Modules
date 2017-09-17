using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface
{
    [Description("Intent HttpServiceProxy - Interceptor Interface")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpProxyInterceptorInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpProxyInterceptorInterfaceTemplate(project);
        }
    }
}

