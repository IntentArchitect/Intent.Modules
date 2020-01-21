using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.Decorators
{
    [Description(DDDEntityInterfaceDecorator.Id)]
    public class DDDEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceTemplate, DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => DDDEntityInterfaceDecorator.Id;

        public override DomainEntityInterfaceDecoratorBase CreateDecoratorInstance(DomainEntityInterfaceTemplate template, IApplication application)
        {
            return new DDDEntityInterfaceDecorator(template);
        }
    }
}
