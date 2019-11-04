using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public interface IEnumModel : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IElementApplication Application { get; }

        IList<IEnumLiteralModel> Literals { get; }

        string Comment { get; }
    }
}