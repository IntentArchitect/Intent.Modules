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
    [Description("Serializable Entity Decorator - Domain decorator")]
    public class SerializableEntityDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return SerializableEntityDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityDecorator();
        }
    }
}
