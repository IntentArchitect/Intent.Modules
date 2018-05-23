using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(SurrogatePrimaryKeyStateDecorator.Identifier)]
    public class SurrogatePrimaryKeyDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyStateDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new SurrogatePrimaryKeyStateDecorator();
        }
    }
}
