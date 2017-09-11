using System.ComponentModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Packages.HttpServiceProxy.Templates.AddressResolverInterface
{
    [Description("Intent HttpServiceProxy - Address Resolver Interface")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => HttpServiceProxyAddressResolverInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new HttpServiceProxyAddressResolverInterfaceTemplate(project);
        }
    }
}
