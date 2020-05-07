using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public interface IServiceProxyModel : IHasStereotypes, IMetadataModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
        ServiceModel MappedService { get; }
        IEnumerable<OperationModel> Operations { get; }
    }
}