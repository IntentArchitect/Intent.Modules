using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public interface IServiceProxyModel : IHasStereotypes, IMetaModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
        IServiceModel MappedService { get; }
        IEnumerable<IOperation> Operations { get; }
    }
}