﻿#nullable enable
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Comment => InternalElement.Comment;
    ITypeReference? ReturnType => InternalElement.TypeReference;
    HttpVerb Verb { get; }
    string Route => $"{BaseRoute?.Trim('/')}/{SubRoute?.Trim('/')}".Trim('/');
    string? BaseRoute { get; }
    string? SubRoute { get; }
    HttpMediaType? MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IElement InternalElement { get; }
    IReadOnlyCollection<IHttpEndpointInputModel> Inputs { get; }
}