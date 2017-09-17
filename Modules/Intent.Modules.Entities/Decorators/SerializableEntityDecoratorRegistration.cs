using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Entities.Decorators
{
    [Description("Serializable Entity Decorator - Domain decorator")]
    public class SerializableEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return SerializableEntityDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityDecorator();
        }
    }
}
