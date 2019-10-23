using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IEnum : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IElementApplication Application { get; }

        IList<IEnumLiteral> Literals { get; }

        string Comment { get; }
    }
}