using Intent.Packages.Entities.Templates.DomainEntity;
using Intent.Packages.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Entities.Decorators
{
    [Description("DDD Entity Interface Decorator - Domain decorator")]
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
