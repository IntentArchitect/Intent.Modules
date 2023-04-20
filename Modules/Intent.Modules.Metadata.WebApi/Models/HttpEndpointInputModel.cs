﻿#nullable enable
using Intent.Metadata.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

internal class HttpEndpointInputModel : IHttpEndpointInputModel
{
    public HttpEndpointInputModel(
        string id,
        string name,
        ITypeReference typeReference,
        HttpInputSource? source,
        string? headerName,
        ICanBeReferencedType? mappedPayloadProperty)
    {
        Name = name;
        TypeReference = typeReference;
        Id = id;
        Source = source;
        HeaderName = headerName;
        MappedPayloadProperty = mappedPayloadProperty;
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public string Id { get; }
    public HttpInputSource? Source { get; }
    public string? HeaderName { get; }
    public ICanBeReferencedType? MappedPayloadProperty { get; }
}