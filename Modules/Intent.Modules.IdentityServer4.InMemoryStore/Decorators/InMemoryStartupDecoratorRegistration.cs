using Intent.Engine;
using Intent.Modules.Common.IdentityServer4.Decorators;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryStartupDecoratorRegistration : DecoratorRegistration<StartupDecorator>
    {
        public override string DecoratorId => InMemoryStartupDecorator.Identifier;

        public override StartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new InMemoryStartupDecorator();
        }
    }
}
