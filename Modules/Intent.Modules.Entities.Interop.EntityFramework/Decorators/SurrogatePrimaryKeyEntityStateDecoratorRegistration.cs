using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    [Description(SurrogatePrimaryKeyEntityStateDecorator.Identifier)]
    public class SurrogatePrimaryKeyEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SurrogatePrimaryKeyEntityStateDecorator();
        }
    }
}
