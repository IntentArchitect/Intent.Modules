using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Angular.Api
{
    public interface IServiceProxyModel : IHasStereotypes, IMetaModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
        IEnumerable<IOperation> Operations { get; }
    }
}