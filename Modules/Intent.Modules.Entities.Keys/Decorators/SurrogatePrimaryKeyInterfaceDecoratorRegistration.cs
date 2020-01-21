using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(SurrogatePrimaryKeyInterfaceDecorator.Identifier)]
    public class SurrogatePrimaryKeyInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceTemplate, DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyInterfaceDecorator.Identifier;
        public override DomainEntityInterfaceDecoratorBase CreateDecoratorInstance(DomainEntityInterfaceTemplate template, IApplication application)
        {
            return new SurrogatePrimaryKeyInterfaceDecorator(template);
        }
    }
}
