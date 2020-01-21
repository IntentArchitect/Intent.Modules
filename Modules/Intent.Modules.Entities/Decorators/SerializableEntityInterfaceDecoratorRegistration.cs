using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;

namespace Intent.Modules.Entities.Decorators
{
    [Description(SerializableEntityInterfaceDecorator.Identifier)]
    public class SerializableEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceTemplate, DomainEntityInterfaceDecoratorBase>
    {

        public override string DecoratorId => SerializableEntityInterfaceDecorator.Identifier;

        public override DomainEntityInterfaceDecoratorBase CreateDecoratorInstance(DomainEntityInterfaceTemplate template, IApplication application)
        {
            return new SerializableEntityInterfaceDecorator(template);
        }
    }
}
