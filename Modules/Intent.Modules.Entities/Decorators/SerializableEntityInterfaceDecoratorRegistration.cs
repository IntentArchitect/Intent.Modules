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
    [Description("Serializable Entity Interface Decorator - Domain decorator")]
    public class SerializableEntityInterfaceDecoratorRegistration : DecoratorRegistration<AbstractDomainEntityInterfaceDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return SerializableEntityInterfaceDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new SerializableEntityInterfaceDecorator();
        }
    }
}
