using Intent.Modules.Entities.Templates.DomainEntity;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityStateDecorator.Identifier)]
    public class SerializableEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {

        public override string DecoratorId => SerializableEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new SerializableEntityStateDecorator(template);
        }
    }
}
