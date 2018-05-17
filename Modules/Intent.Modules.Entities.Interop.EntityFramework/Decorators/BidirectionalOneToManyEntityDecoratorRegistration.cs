using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(BidirectionalOneToManyEntityDecorator.Identifier)]
    public class BidirectionalOneToManyEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {
        public override string DecoratorId => BidirectionalOneToManyEntityDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new BidirectionalOneToManyEntityDecorator();
        }
    }
}
