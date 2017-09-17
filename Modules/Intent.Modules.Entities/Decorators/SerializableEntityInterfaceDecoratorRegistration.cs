using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Entities.Decorators
{
    [Description("Serializable Entity Interface Decorator - Domain decorator")]
    public class SerializableEntityInterfaceDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityInterfaceDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return SerializableEntityInterfaceDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityInterfaceDecorator();
        }
    }
}
