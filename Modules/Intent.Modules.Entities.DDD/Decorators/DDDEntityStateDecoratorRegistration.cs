using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityStateDecorator.Identifier)]
    public class DDDEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => DDDEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new DDDEntityStateDecorator(template);
        }
    }
}
