using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description("Intent.Entities.Interop.EntityFramework - BidirectionalOneToManyEntityDecorator")]
    public class BidirectionalOneToManyEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {
        public override string DecoratorId => BidirectionalOneToManyEntityDecorator.Id;

        public override object CreateDecoratorInstance()
        {
            return new BidirectionalOneToManyEntityDecorator();
        }
    }
}
