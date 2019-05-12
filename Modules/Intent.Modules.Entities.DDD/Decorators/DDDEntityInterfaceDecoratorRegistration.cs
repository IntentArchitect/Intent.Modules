using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Entities.Decorators
{
    [Description(DDDEntityInterfaceDecorator.Id)]
    public class DDDEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => DDDEntityInterfaceDecorator.Id;

        public override DomainEntityInterfaceDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new DDDEntityInterfaceDecorator();
        }
    }
}
