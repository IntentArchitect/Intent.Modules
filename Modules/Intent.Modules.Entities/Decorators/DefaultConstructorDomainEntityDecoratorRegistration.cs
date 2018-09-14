using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;

namespace Intent.Modules.Entities.Decorators
{
    public class DefaultConstructorDomainEntityDecoratorRegistration : DecoratorRegistration<DomainEntityDecoratorBase>
    {
        public override string DecoratorId => DefaultConstructorDomainEntityDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new DefaultConstructorDomainEntityDecorator();
        }
    }
}
