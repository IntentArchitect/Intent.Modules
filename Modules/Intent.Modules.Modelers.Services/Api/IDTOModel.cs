using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modelers.Services.Api
{
    public interface IDTOModel : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IEnumerable<string> GenericTypes { get; } 

        bool IsMapped { get; }

        IElementMapping MappedClass { get; }

        IElementApplication Application { get; }

        IEnumerable<IAttribute> Fields { get; }

        string Comment { get; }
    }
}