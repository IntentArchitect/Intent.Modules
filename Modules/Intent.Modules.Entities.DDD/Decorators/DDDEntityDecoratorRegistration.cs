using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityDecorator.Identifier)]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<DomainEntityDecoratorBase>
    {
        public override string DecoratorId => DDDEntityDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new DDDEntityDecorator();
        }
    }
}
