using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Entities.Decorators
{
    [Description("DDD Entity Decorator - Domain decorator")]
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
