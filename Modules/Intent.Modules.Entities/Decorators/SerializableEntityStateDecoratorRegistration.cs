using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityStateDecorator.Identifier)]
    public class SerializableEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {

        public override string DecoratorId => SerializableEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SerializableEntityStateDecorator();
        }
    }
}
