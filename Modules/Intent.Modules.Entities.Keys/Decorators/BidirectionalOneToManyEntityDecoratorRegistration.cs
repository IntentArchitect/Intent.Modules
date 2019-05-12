using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(BidirectionalOneToManyEntityStateDecorator.Identifier)]
    public class BidirectionalOneToManyEntityDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => BidirectionalOneToManyEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new BidirectionalOneToManyEntityStateDecorator(template);
        }
    }
}
