using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IEnumLiteral : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string Value { get; }
        string Comment { get; }
    }
}