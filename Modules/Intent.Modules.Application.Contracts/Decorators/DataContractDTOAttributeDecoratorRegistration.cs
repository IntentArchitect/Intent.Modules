using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;
using Intent.Packages.Application.Contracts.Templates.DTO;
using System.ComponentModel;

namespace Intent.Packages.Application.Contracts.Decorators
{
    [Description("Intent Applications Contracts DTO - Data Contract decorator")]
    public class DataContractDTOAttributeDecoratorRegistration : DecoratorRegistration<IDTOAttributeDecorator>
    {
        public override string DecoratorId
        {
            get
            {
                return DataContractDTOAttributeDecorator.Id;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new DataContractDTOAttributeDecorator();
        }
    }
}
