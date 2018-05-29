using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(SurrogatePrimaryKeyInterfaceDecorator.Identifier)]
    public class SurrogatePrimaryKeyInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyInterfaceDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new SurrogatePrimaryKeyInterfaceDecorator();
        }
    }
}
