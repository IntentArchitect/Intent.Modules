using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(ForeignKeyEntityInterfaceDecorator.Identifier)]
    public class ForeignKeyEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => ForeignKeyEntityInterfaceDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new ForeignKeyEntityInterfaceDecorator();
        }
    }
}
