using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityDecorator.Identifier)]
    public class SerializableEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {

        public override string DecoratorId => SerializableEntityDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityDecorator();
        }
    }
}
