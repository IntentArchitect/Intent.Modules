using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Entities.Decorators
{
    [Description("Intent.Entities.DDD.EntityInterfaceDecorator")]
    public class DDDEntityInterfaceDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityInterfaceDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return DDDEntityInterfaceDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new DDDEntityInterfaceDecorator();
        }
    }
}
