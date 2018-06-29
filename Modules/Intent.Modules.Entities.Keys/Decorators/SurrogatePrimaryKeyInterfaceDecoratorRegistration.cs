using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(SurrogatePrimaryKeyInterfaceDecorator.Identifier)]
    public class SurrogatePrimaryKeyInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyInterfaceDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SurrogatePrimaryKeyInterfaceDecorator();
        }
    }
}
