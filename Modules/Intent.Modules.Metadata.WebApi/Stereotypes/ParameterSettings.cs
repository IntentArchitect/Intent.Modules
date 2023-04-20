#nullable enable
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Metadata.WebApi.Stereotypes;

public class ParameterSettings
{
    public HttpInputSource? Source { get; set; }
    public string? HeaderName { get; set; }
}