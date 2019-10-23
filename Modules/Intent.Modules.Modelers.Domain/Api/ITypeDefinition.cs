using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface ITypeDefinition : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IEnumerable<string> GenericTypes { get; }

        IElementApplication Application { get; }

        string Comment { get; }
    }
}