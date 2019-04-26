using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(BidirectionalOneToManyEntityStateDecorator.Identifier)]
    public class BidirectionalOneToManyEntityDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => BidirectionalOneToManyEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new BidirectionalOneToManyEntityStateDecorator();
        }
    }
}
