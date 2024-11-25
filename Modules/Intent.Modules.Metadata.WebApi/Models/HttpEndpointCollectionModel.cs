#nullable enable
using System.Collections.Generic;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

internal class HttpEndpointCollectionModel : IHttpEndpointCollectionModel
{
    public HttpEndpointCollectionModel(
        bool securedByDefault,
        IReadOnlyCollection<ISecurityModel> securityModels,
        IReadOnlyCollection<IHttpEndpointModel> endpoints,
        bool allowAnonymous)
    {
        SecuredByDefault = securedByDefault;
        SecurityModels = securityModels;
        Endpoints = endpoints;
        AllowAnonymous = allowAnonymous;
    }

    public bool SecuredByDefault { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public IReadOnlyCollection<IHttpEndpointModel> Endpoints { get; }
    public bool AllowAnonymous { get; }
}