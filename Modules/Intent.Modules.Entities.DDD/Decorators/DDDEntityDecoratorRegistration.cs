using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Engine;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityDecorator.Identifier)]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<DomainEntityDecoratorBase>
    {
        public override string DecoratorId => DDDEntityDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new DDDEntityDecorator();
        }
    }
}
