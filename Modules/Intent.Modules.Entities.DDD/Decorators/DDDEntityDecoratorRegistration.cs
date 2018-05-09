using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Entities.Decorators
{
    [Description("Intent.Entities.DDD.EntityDecorator")]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return DDDEntityDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new DDDEntityDecorator();
        }
    }
}
