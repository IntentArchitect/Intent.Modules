using Intent.Engine;
using System;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Templates;

namespace Intent.Modules.Entities.Decorators
{
    public class OrphanDetectionDomainEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => OrphanDetectionDomainEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new OrphanDetectionDomainEntityStateDecorator(template);
        }
    }
}
