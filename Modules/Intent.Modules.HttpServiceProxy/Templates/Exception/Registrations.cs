using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.HttpServiceProxy.Templates.Exception
{
    [Description("Intent HttpServiceProxy - Remote Exception")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => WebApiClientServiceProxyRemoteExceptionTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new WebApiClientServiceProxyRemoteExceptionTemplate(project);
        }
    }
}

