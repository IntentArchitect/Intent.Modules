using Intent.MetaModel.Service;
using Intent.Templates

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    public interface IServiceContractAttributeDecorator : ITemplateDecorator
    {
        string ContractAttributes(IServiceModel service);
        string OperationAttributes(IServiceModel service, IOperationModel operation);
    }
}
