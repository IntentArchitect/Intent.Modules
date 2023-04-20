using Intent.Metadata.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointInputModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    ICanBeReferencedType? MappedPayloadProperty { get; }
}