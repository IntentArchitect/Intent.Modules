using Intent.Engine;
using Intent.Modules.Common.IdentityServer4.Decorators;
using Intent.Modules.Common.Registrations;
using System;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryIdentityConfigDecoratorRegistration : DecoratorRegistration<IdentityConfigDecorator>
    {
        public override string DecoratorId => InMemoryIdentityConfigDecorator.Identifier;

        public override IdentityConfigDecorator CreateDecoratorInstance(IApplication application)
        {
            return new InMemoryIdentityConfigDecorator();
        }
    }
}
