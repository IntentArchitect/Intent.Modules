#nullable enable
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Metadata.WebApi.Models;

public interface IHttpEndpointModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Comment => InternalElement.Comment;
    ITypeReference? ReturnType => InternalElement.TypeReference;
    HttpVerb Verb { get; }
    string Route
    {
        get
        {
            // It's a smell that we're applying a C# WebApi convention for this which is intended
            // to be consumed by any kind of technology, but unlikely to cause an issue since
            // [controller]/[action] convention doesn't seem to be used elsewhere that we're aware of.
            var serviceName = (InternalElement.ParentElement?.Name ?? "Default").RemoveSuffix("Controller", "Service");
            var actionName = InternalElement.Name;
            return $"{BaseRoute?.Trim('/')}/{SubRoute?.Trim('/')}"
                .Trim('/')
                .Replace("[controller]", serviceName)
                .Replace("[action]", actionName)
                .ToLowerInvariant();
        }
    }

    string? BaseRoute { get; }
    string? SubRoute { get; }
    HttpMediaType? MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IElement InternalElement { get; }
    IReadOnlyCollection<IHttpEndpointInputModel> Inputs { get; }
}