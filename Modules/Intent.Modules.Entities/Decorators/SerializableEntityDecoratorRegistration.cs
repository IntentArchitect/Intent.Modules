using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityStateDecorator.Identifier)]
    public class SerializableEntityDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {

        public override string DecoratorId => SerializableEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityStateDecorator();
        }
    }
}
