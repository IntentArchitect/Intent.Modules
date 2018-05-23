using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(BidirectionalOneToManyEntityStateDecorator.Identifier)]
    public class BidirectionalOneToManyEntityDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => BidirectionalOneToManyEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new BidirectionalOneToManyEntityStateDecorator();
        }
    }
}
