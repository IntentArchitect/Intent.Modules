#nullable enable
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointModel : IHasName, IHasTypeReference, IMetadataModel, IElementWrapper
{
    string Comment => InternalElement.Comment;
    ITypeReference? ReturnType => InternalElement.TypeReference;
    HttpVerb Verb { get; }
    string Route { get; }
    string? BaseRoute { get; }
    string? SubRoute { get; }
    HttpMediaType? MediaType { get; }
    bool SecuredByDefault { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    bool RequiresAuthorization => SecurityModels?.Count > 0 || (SecuredByDefault && !AllowAnonymous);
    bool AllowAnonymous { get; }
    new IElement InternalElement => ((IElementWrapper)this).InternalElement;
    IReadOnlyCollection<IHttpEndpointInputModel> Inputs { get; }
}