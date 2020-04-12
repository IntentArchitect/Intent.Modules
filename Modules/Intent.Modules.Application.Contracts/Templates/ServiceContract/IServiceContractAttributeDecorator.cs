using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    public interface IServiceContractAttributeDecorator : ITemplateDecorator
    {
        string ContractAttributes(ServiceModel service);
        string OperationAttributes(ServiceModel service, OperationModel operation);
    }
}
