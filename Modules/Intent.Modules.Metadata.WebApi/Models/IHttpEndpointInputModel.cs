#nullable enable
using Intent.Metadata.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointInputModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    string? QueryStringName { get; }
    ICanBeReferencedType? MappedPayloadProperty { get; }
    string? Value { get; }
}