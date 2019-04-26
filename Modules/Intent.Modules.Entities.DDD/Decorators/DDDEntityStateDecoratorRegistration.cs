using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityStateDecorator.Identifier)]
    public class DDDEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => DDDEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new DDDEntityStateDecorator();
        }
    }
}
