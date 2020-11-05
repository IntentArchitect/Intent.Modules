using Intent.Engine;
using Intent.Modules.Common.IdentityServer4.Decorators;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class X509CertSigningStartupDecoratorRegistration : DecoratorRegistration<StartupDecorator>
    {
        public override string DecoratorId => X509CertSigningStartupDecorator.Identifier;

        public override StartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new X509CertSigningStartupDecorator();
        }
    }
}
