#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Stereotypes;

namespace Intent.Modules.Metadata.WebApi.Models;
public static class HttpEndpointModelFactory
{
    public static IHttpEndpointModel? GetEndpoint(IElement element)
    {
        return GetEndpoint(element, null);
    }

    public static IHttpEndpointModel? GetEndpoint(IElement element, string? defaultBasePath)
    {
        if (element.Stereotypes.Any(s => s.Name == "Http Settings (Obsolete)"))
        {
            throw new Exception("A migration is outstanding on the services designer. Please open the services designer which will run the migration automatically and be sure to press save regardless afterwards.");
        }

        if (!element.TryGetHttpSettings(out var httpSettings))
        {
            return null;
        }

        var hasSecured = element.TryGetSecured(out _);
        var hasUnsecured = element.TryGetUnsecured(out _);
        var hasAuthorize = element.TryGetAuthorize(out _);
        if ((hasSecured || hasAuthorize) && hasUnsecured)
        {
            throw new Exception($"{element.Name} [{element.Id}] cannot require authorization and allow-anonymous at the same time");
        }

        var baseRoute = GetBaseRoute(element, defaultBasePath);
        var subRoute = httpSettings!.Route;

        return new HttpEndpointModel(
            name: element.SpecializationTypeId switch
            {
                // The `.RemoveSuffix("Request")`s below are not merged and in a specific order
                // because we do still want names like "SomeRequestCommand" / "SomeRequestQuery".
                Constants.ElementTypeIds.Command => element.Name
                    .RemoveSuffix("Request")
                    .RemoveSuffix("Command"),
                Constants.ElementTypeIds.Query => element.Name
                    .RemoveSuffix("Request")
                    .RemoveSuffix("Query"),
                _ => element.Name
            },
            verb: httpSettings.Verb ?? element.SpecializationTypeId switch
            {
                Constants.ElementTypeIds.Operation => HttpVerb.Post,
                Constants.ElementTypeIds.Command => HttpVerb.Post,
                Constants.ElementTypeIds.Query => HttpVerb.Get,
                _ => throw new InvalidOperationException($"Unknown type: \"{element.SpecializationType}\" ({element.SpecializationTypeId})")
            },
            route: GetRoute(element, baseRoute, subRoute),
            baseRoute: baseRoute,
            subRoute: subRoute,
            mediaType: httpSettings.ReturnTypeMediatype,
            requiresAuthorization: hasSecured || hasAuthorize,
            allowAnonymous: hasUnsecured,
            internalElement: element,
            inputs: GetParameters(element, httpSettings).ToArray());
    }

    private static string GetRoute(IElement element, string? baseRoute, string? subRoute)
    {
        // It's a smell that we're applying C# WebApi conventions for this which is intended
        // to be consumed by any kind of technology, but unlikely to cause an issue since
        // [controller]/[action] convention doesn't seem to be used elsewhere that we're aware of.
        var serviceName = (element.ParentElement?.Name ?? "Default").RemoveSuffix("Controller", "Service");
        var actionName = element.Name;

        // "~/" at the start of the route means to ignore the controller route:
        // https://learn.microsoft.com/aspnet/core/mvc/controllers/routing#attribute-routing-for-rest-apis
        if (subRoute?.StartsWith("~/") == true)
        {
            baseRoute = null;
            subRoute = subRoute[2..];
        }

        var routeConstruction = $"{baseRoute?.Trim('/')}/{subRoute?.Trim('/')}"
            .Trim('/')
            .Replace("[controller]", serviceName)
            .Replace("[action]", actionName);

        if (element.TryGetApiVersion(out var apiVersion))
        {
            var version = apiVersion!.ApplicableVersions.Max(x => x.Version);
            version = FormatVersion(version);
            routeConstruction = routeConstruction.Replace("{version}", version);
        }
        else if (element.ParentElement?.TryGetApiVersion(out var parentApiVersion) == true)
        {
            var version = parentApiVersion!.ApplicableVersions.Max(x => x.Version);
            version = FormatVersion(version);
            routeConstruction = routeConstruction.Replace("{version}", version);
        }

        return routeConstruction.ToLowerInvariant();
    }

    private static string FormatVersion(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return version;
        }

        if (!version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            version = "v" + version;
        }

        var components = version.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (components.Length <= 1)
        {
            return version;
        }

        var mutableComponents = components.ToList();
        for (var index = components.Length - 1; index > 0; index--)
        {
            if (components[index] == "0")
            {
                mutableComponents.RemoveAt(index);
            }
            else
            {
                break;
            }
        }

        return string.Join(".", mutableComponents);
    }

    public static HttpInputSource? GetHttpInputSource(IElement childElement)
    {
        if (!childElement.ParentElement.TryGetHttpSettings(out var httpSettings))
        {
            return null;
        }

        if ((childElement.TypeReference.Element.IsTypeDefinitionModel() || childElement.TypeReference.Element.IsEnumModel()) &&
                httpSettings!.Route?.ToLower().Contains($"{{{childElement.Name.ToLower()}}}") == true)
        {
            return HttpInputSource.FromRoute;
        }

        if (httpSettings!.Verb is HttpVerb.Get or HttpVerb.Delete)
        {
            return HttpInputSource.FromQuery;
        }

        if (httpSettings.Verb is HttpVerb.Post or HttpVerb.Put &&
            !(childElement.TypeReference.Element.IsTypeDefinitionModel() || childElement.TypeReference.Element.IsEnumModel()))
        {
            return HttpInputSource.FromBody;
        }

        return null;
    }

    private static string? GetBaseRoute(IElement element, string? defaultBasePath)
    {
        var baseRoute = element.ParentElement?.TryGetHttpServiceSettings(out var serviceSettings) == true &&
                        !string.IsNullOrWhiteSpace(serviceSettings!.Route)
            ? serviceSettings.Route
            : defaultBasePath;

        if (baseRoute == null)
        {
            baseRoute = string.Empty;
        }

        return baseRoute;
    }

    private static IEnumerable<IHttpEndpointInputModel> GetParameters(
        IElement element,
        HttpSettings httpSettings)
    {
        var isForCqrs = element.SpecializationTypeId is Constants.ElementTypeIds.Query or Constants.ElementTypeIds.Command;
        var verbAllowsBody = httpSettings.Verb is HttpVerb.Put or HttpVerb.Patch or HttpVerb.Post;
        var requiresBody = false;

        foreach (var childElement in element.ChildElements)
        {
            var hasParameterSettings = childElement.TryGetParameterSettings(out var parameterSettings);
            var routeContainsParameter = httpSettings.Route?.ToLower().Contains($"{{{childElement.Name.ToLower()}}}") == true;

            if (isForCqrs && !hasParameterSettings && !routeContainsParameter && verbAllowsBody)
            {
                requiresBody = true;
                continue;
            }

            yield return new HttpEndpointInputModel(
                id: childElement.Id,
                name: childElement.Name.ToCamelCase(),
                typeReference: childElement.TypeReference,
                source: parameterSettings?.Source ?? GetHttpInputSource(childElement),
                headerName: parameterSettings?.HeaderName,
                queryStringName: parameterSettings?.QueryStringName,
                mappedPayloadProperty: childElement,
                value: childElement.Value);
        }

        if (isForCqrs && requiresBody)
        {
            yield return new HttpEndpointInputModel(
                id: element.Id,
                name: element.SpecializationTypeId switch
                {
                    Constants.ElementTypeIds.Command => "command",
                    Constants.ElementTypeIds.Query => "query",
                    _ => throw new InvalidOperationException($"Unknown type: \"{element.SpecializationType}\" ({element.SpecializationTypeId})")
                },
                typeReference: element.AsTypeReference(),
                source: HttpInputSource.FromBody,
                headerName: null,
                queryStringName: null,
                mappedPayloadProperty: null,
                value: null);
        }
    }
}