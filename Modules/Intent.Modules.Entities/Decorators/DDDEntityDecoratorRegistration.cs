using Intent.Packages.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Entities.Decorators
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
