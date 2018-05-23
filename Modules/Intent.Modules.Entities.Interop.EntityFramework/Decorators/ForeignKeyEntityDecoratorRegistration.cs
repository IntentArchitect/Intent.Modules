using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(ForeignKeyEntityStateDecorator.Identifier)]
    public class ForeignKeyEntityDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => ForeignKeyEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new ForeignKeyEntityStateDecorator();
        }
    }
}
