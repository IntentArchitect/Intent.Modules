using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class OrphanDetectionDomainEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => OrphanDetectionDomainEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new OrphanDetectionDomainEntityStateDecorator();
        }
    }
}
