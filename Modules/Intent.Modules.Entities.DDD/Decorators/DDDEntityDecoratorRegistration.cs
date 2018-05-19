using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    [Description(DDDEntityDecorator.Identifier)]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {

        public override string DecoratorId => DDDEntityDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new DDDEntityDecorator();
        }
    }
}
