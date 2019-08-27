using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public interface IServiceModel : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        string ApplicationName { get; }

        IElementApplication Application { get; }

        IEnumerable<IOperation> Operations { get; }

        string Comment { get; }
    }
}