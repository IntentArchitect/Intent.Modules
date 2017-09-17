using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.HttpServiceProxy.Templates.AddressResolverImplementation
{
    [Description("Intent HttpServiceProxy - Address Resolver Implementation")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpServiceProxyAddressResolverImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpServiceProxyAddressResolverImplementationTemplate(project);
        }
    }
}
