#nullable enable
using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modules.Common;
using Intent.Modules.Metadata.WebApi.Stereotypes;

namespace Intent.Modules.Metadata.WebApi.Models;

public static class ElementExtensionMethods
{
    public static bool TryGetHttpSettings(this IElement element, out HttpSettings? httpSettings)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Http Settings");
        if (stereotype == null)
        {
            httpSettings = null;
            return false;
        }

        var route = stereotype.GetProperty<string>("Route");
        var verb = stereotype.GetProperty<string>("Verb");
        var returnTypeMediatype = stereotype.GetProperty<string>("Return Type Mediatype")?
            .Replace("default", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("/", string.Empty); // Can have values like "application/json"

        httpSettings = new HttpSettings
        {
            Route = route,
            Verb = string.IsNullOrWhiteSpace(verb) ? null : Enum.Parse<HttpVerb>(verb, ignoreCase: true),
            ReturnTypeMediatype = string.IsNullOrWhiteSpace(returnTypeMediatype) ? null : Enum.Parse<HttpMediaType>(returnTypeMediatype, ignoreCase: true)
        };
        return true;
    }

    public static bool TryGetHttpServiceSettings(this IElement element, out HttpServiceSettings? httpServiceSettings)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Http Service Settings");
        if (stereotype == null)
        {
            httpServiceSettings = null;
            return false;
        }

        httpServiceSettings = new HttpServiceSettings
        {
            Route = stereotype.GetProperty<string>("Route")
        };

        return true;
    }

    public static bool TryGetSecured(this IElement element, out Secured? secured)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Secured");
        if (stereotype == null)
        {
            secured = null;
            return false;
        }

        secured = new Secured
        {
            Roles = stereotype.Properties.SingleOrDefault(x => x.Key == "Roles")?.Value
        };

        return true;
    }

    public static bool TryGetUnsecured(this IElement element, out Unsecured? unsecured)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Unsecured");
        if (stereotype == null)
        {
            unsecured = null;
            return false;
        }

        unsecured = new Unsecured();
        return true;
    }

    public static bool TryGetAuthorize(this IElement element, out Authorize? authorize)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Authorize");
        if (stereotype == null)
        {
            authorize = null;
            return false;
        }

        authorize = new Authorize();
        return true;
    }

    public static bool TryGetParameterSettings(this IElement element, out ParameterSettings? parameterSettings)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Parameter Settings");
        if (stereotype == null)
        {
            parameterSettings = null;
            return false;
        }

        var source = stereotype.GetProperty<string>("Source");
        var headerName = stereotype.GetProperty<string>("Header Name");

        parameterSettings = new ParameterSettings
        {
            Source = string.IsNullOrWhiteSpace(source) || string.Equals(source, "default", StringComparison.OrdinalIgnoreCase)
                ? null
                : Enum.Parse<HttpInputSource>(source.Replace(" ", ""), ignoreCase: true),
            HeaderName = headerName
        };
        return true;
    }

    public static bool TryGetApiVersion(this IElement element, out ApiVersion? apiVersion)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.Name == "Api Version");
        if (stereotype == null)
        {
            apiVersion = null;
            return false;
        }

        var applicableVersionElements = stereotype.GetProperty<IElement[]>("Applicable Versions") 
                                        ?? Array.Empty<IElement>();
        var versions = applicableVersionElements
            .Select(s => s.AsVersionModel())
            .Where(p => p is not null)
            .ToArray();

        if (versions.Length == 0)
        {
            apiVersion = null;
            return false;
        }

        apiVersion = new ApiVersion
        {
            ApplicableVersions = versions.Select(s => new ApiApplicableVersion
            {
                DefinitionName = s.VersionDefinition?.Name,
                Version = s.Name,
                IsDeprecated = s.GetVersionSettings()?.IsDeprecated() == true
            }).ToList()
        };
        return true;
    }
}