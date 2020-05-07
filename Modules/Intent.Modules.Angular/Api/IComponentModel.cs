using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public interface IComponentModel : IHasStereotypes, IMetadataModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
        IEnumerable<AttributeModel> Models { get; }
        IEnumerable<OperationModel> Commands { get; }
    }
}