using Intent.Modules.Application.Contracts.Templates.DTO;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Decorators
{
    [Description("Intent Applications Contracts DTO - Data Contract decorator")]
    public class DataContractDTOAttributeDecoratorRegistration : DecoratorRegistration<IDTOAttributeDecorator>
    {
        public override string DecoratorId => DataContractDTOAttributeDecorator.Id;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new DataContractDTOAttributeDecorator();
        }
    }
}
