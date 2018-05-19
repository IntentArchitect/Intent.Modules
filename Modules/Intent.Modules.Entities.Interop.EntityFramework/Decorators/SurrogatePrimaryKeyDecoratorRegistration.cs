using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(SurrogatePrimaryKeyDecorator.Identifier)]
    public class SurrogatePrimaryKeyDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {
        public override string DecoratorId => SurrogatePrimaryKeyDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new SurrogatePrimaryKeyDecorator();
        }
    }
}
