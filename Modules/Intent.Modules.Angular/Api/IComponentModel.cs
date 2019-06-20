using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Angular.Api
{
    public interface IComponentModel : IHasStereotypes, IMetadataModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
        IEnumerable<IAttribute> Models { get; }
        IEnumerable<IOperation> Commands { get; }
    }
}