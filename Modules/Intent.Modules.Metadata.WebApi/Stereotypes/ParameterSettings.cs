#nullable enable
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Metadata.WebApi.Stereotypes;

public class ParameterSettings
{
    public ParameterSettings(HttpInputSource? source, string? headerName, string? queryStringName)
    {
        Source = source;
        HeaderName = headerName;
        QueryStringName = queryStringName;
    }

    public HttpInputSource? Source { get; }
    public string? HeaderName { get; }
    public string? QueryStringName { get; }
}