using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityDecorator.Identifier)]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<DomainEntityTemplate, DomainEntityDecoratorBase>
    {
        public override string DecoratorId => DDDEntityDecorator.Identifier;

        public override DomainEntityDecoratorBase CreateDecoratorInstance(DomainEntityTemplate template, IApplication application)
        {
            return new DDDEntityDecorator(template);
        }
    }
}
