#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

internal class HttpEndpointModel : IHttpEndpointModel
{
    public HttpEndpointModel(
        string? name,
        HttpVerb verb,
        string route,
        string? baseRoute,
        string? subRoute,
        HttpMediaType? mediaType,
        IReadOnlyCollection<ISecurityModel> securityModels,
        bool securedByDefault,
        bool allowAnonymous,
        IElement internalElement,
        IReadOnlyCollection<IHttpEndpointInputModel> inputs)
    {
        if (verb is not (HttpVerb.Patch or HttpVerb.Post or HttpVerb.Put) &&
            inputs.Any(x => x.Source == HttpInputSource.FromBody))
        {
            throw new ElementException(internalElement, $"Cannot set an HTTP endpoint's source to \"body\" when it has a verb of {verb.ToString().ToUpperInvariant()}");
        }

        Name = name;
        Verb = verb;
        Route = route;
        BaseRoute = baseRoute;
        SubRoute = subRoute;
        MediaType = mediaType;
        AllowAnonymous = allowAnonymous;
        InternalElement = internalElement;
        Inputs = inputs;
        SecuredByDefault = securedByDefault;
        SecurityModels = securityModels;
    }

    public string Id => InternalElement.Id;
    public string? Name { get; }
    public ITypeReference? TypeReference => InternalElement.TypeReference;
    public HttpVerb Verb { get; }
    public string Route { get; }
    public string? BaseRoute { get; }
    public string? SubRoute { get; }
    public HttpMediaType? MediaType { get; }
    public bool SecuredByDefault { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public bool AllowAnonymous { get; }
    public IElement InternalElement { get; }
    public IReadOnlyCollection<IHttpEndpointInputModel> Inputs { get; }
}