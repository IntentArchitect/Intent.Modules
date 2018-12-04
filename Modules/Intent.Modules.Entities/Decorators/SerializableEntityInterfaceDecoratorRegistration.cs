using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityInterfaceDecorator.Identifier)]
    public class SerializableEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {

        public override string DecoratorId => SerializableEntityInterfaceDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SerializableEntityInterfaceDecorator();
        }
    }
}
