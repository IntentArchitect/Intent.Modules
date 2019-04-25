using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;

namespace Intent.Modules.Entities.Decorators
{
    [Description(DDDEntityInterfaceDecorator.Id)]
    public class DDDEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceDecoratorBase>
    {

        public override string DecoratorId => DDDEntityInterfaceDecorator.Id;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new DDDEntityInterfaceDecorator();
        }
    }
}
