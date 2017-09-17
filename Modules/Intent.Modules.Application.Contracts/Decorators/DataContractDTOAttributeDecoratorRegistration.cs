using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Application.Contracts.Decorators
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
