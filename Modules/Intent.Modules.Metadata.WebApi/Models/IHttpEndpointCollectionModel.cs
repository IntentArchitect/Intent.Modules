#nullable enable
using System.Collections.Generic;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointCollectionModel
{
    bool SecuredByDefault { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    IReadOnlyCollection<IHttpEndpointModel> Endpoints { get; }
    bool RequiresAuthorization => SecurityModels?.Count > 0 || (SecuredByDefault && !AllowAnonymous);
    bool AllowAnonymous { get; }
}