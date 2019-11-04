using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public interface IEnumLiteralModel : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string Value { get; }
        string Comment { get; }
    }
}