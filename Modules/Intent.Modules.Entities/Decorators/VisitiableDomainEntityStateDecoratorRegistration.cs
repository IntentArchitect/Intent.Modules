using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class VisitiableDomainEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => VisitiableDomainEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new VisitiableDomainEntityStateDecorator();
        }
    }
}
