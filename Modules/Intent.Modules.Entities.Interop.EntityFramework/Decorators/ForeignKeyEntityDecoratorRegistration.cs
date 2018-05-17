using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(ForeignKeyEntityDecorator.Identifier)]
    public class ForeignKeyEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {
        public override string DecoratorId => ForeignKeyEntityDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new ForeignKeyEntityDecorator();
        }
    }
}
