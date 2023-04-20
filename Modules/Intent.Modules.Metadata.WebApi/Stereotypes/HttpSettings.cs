#nullable enable
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Metadata.WebApi.Stereotypes;

public class HttpSettings
{
    public string? Route { get; set; }
    public HttpVerb? Verb { get; set; }
    public HttpMediaType? ReturnTypeMediatype { get; set; }
}