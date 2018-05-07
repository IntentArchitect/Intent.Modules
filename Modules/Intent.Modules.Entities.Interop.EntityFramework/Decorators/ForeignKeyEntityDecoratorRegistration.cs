using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description("Intent.Entities.Interop.EntityFramework - ForeignKeyEntityDecorator")]
    public class ForeignKeyEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {
        public override string DecoratorId => ForeignKeyEntityDecorator.Id;

        public override object CreateDecoratorInstance()
        {
            return new ForeignKeyEntityDecorator();
        }
    }
}
