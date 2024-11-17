#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Stereotypes;

namespace Intent.Modules.Metadata.WebApi.Models;

public static class HttpEndpointModelFactory
{
    public static bool TryGetCollection(
        IElement element,
        string? defaultBasePath,
        bool securedByDefault,
        [NotNullWhen(true)] out IHttpEndpointCollectionModel? httpEndpointCollectionModel)
    {
        var derivedSecurityModels = SecurityModelHelpers.GetSecurityModels(element).ToList();

        return TryGetCollection(
            hasStereotypes: element,
            defaultBasePath: defaultBasePath,
            securedByDefault: securedByDefault,
            derivedSecurityModels: derivedSecurityModels,
            httpEndpointCollectionModel: out httpEndpointCollectionModel);
    }

    public static bool TryGetCollection(
        IPackage package,
        string? defaultBasePath,
        bool securedByDefault,
        [NotNullWhen(true)] out IHttpEndpointCollectionModel? httpEndpointCollectionModel)
    {
        var derivedSecurityModels = SecurityModelHelpers.GetSecurityModels(package).ToList();

        return TryGetCollection(
            hasStereotypes: package,
            defaultBasePath: defaultBasePath,
            securedByDefault: securedByDefault,
            derivedSecurityModels: derivedSecurityModels,
            httpEndpointCollectionModel: out httpEndpointCollectionModel);
    }

    private static bool TryGetCollection(
        IHasStereotypes hasStereotypes,
        string? defaultBasePath,
        bool securedByDefault,
        List<ISecurityModel> derivedSecurityModels,
        [NotNullWhen(true)] out IHttpEndpointCollectionModel? httpEndpointCollectionModel)
    {
        if (securedByDefault && derivedSecurityModels.Count == 0)
        {
            derivedSecurityModels.Add(ISecurityModel.Empty);
        }

        var childElements = hasStereotypes switch
        {
            IPackage hasChildElements => hasChildElements.ChildElements,
            IElement hasChildElements => hasChildElements.ChildElements,
            _ => throw new Exception($"Unsupported type: {hasStereotypes?.GetType()}")
        };

        var endpoints = childElements
            .Select(childElement =>
            {
                var securityModels = SecurityModelHelpers.GetSecurityModels(childElement, checkParents: false).ToList();
                if (securedByDefault)
                {
                    securityModels = securityModels.Where(x => !x.Equals(ISecurityModel.Empty)).ToList();
                }

                var success = TryGetEndpoint(
                    element: childElement,
                    defaultBasePath: defaultBasePath,
                    securedByDefault: securedByDefault || derivedSecurityModels.Count > 0,
                    securityModels: securityModels,
                    httpEndpoint: out var httpEndpointModel);

                return new
                {
                    Success = success,
                    Endpoint = httpEndpointModel!,
                    SecurityModels = securityModels
                };
            })
            .Where(x => x.Success)
            .ToArray();

        if (hasStereotypes is IPackage or IElement { SpecializationTypeId: Constants.ElementTypeIds.Folder } &&
            endpoints.Length == 0)
        {
            httpEndpointCollectionModel = null;
            return false;
        }

        httpEndpointCollectionModel = new HttpEndpointCollectionModel(
            securedByDefault: securedByDefault,
            securityModels: derivedSecurityModels,
            endpoints: endpoints
                .Select(x => GetEndpoint(
                    element: x.Endpoint.InternalElement,
                    defaultBasePath: defaultBasePath,
                    securedByDefault: securedByDefault || derivedSecurityModels.Count > 0,
                    securityModels: x.SecurityModels))
                .ToArray(),
            allowAnonymous: hasStereotypes.HasStereotype(Constants.Stereotypes.Unsecured.Id));
        return true;
    }

    /// <summary>
    /// Obsolete. Use <see cref="TryGetEndpoint"/> instead.
    /// </summary>
    [Obsolete("See XML doc comment")]
    public static IHttpEndpointModel? GetEndpoint(IElement element)
    {
        return TryGetEndpoint(
            element: element,
            defaultBasePath: null,
            securedByDefault: false,
            httpEndpointModel: out var httpEndpoint)
                ? httpEndpoint
                : null;
    }

    /// <summary>
    /// Obsolete. Use <see cref="TryGetEndpoint"/> instead.
    /// </summary>
    [Obsolete("See XML doc comment")]
    public static IHttpEndpointModel? GetEndpoint(IElement element, string? defaultBasePath)
    {
        return TryGetEndpoint(element, defaultBasePath, false, out var httpEndpoint)
            ? httpEndpoint
            : null;
    }

    /// <summary>
    /// Gets details of an endpoint where <see cref="IHttpEndpointModel.SecurityModels"/> contains
    /// both directly and indirectly applied Security stereotypes (i.e. on parent elements
    /// or the package).
    /// </summary>
    /// <param name="element">The element representing the endpoint, works with Operation, Command and Query element types.</param>
    /// <param name="defaultBasePath">The default base API path, typically captured in the application settings.</param>
    /// <param name="securedByDefault">Whether API endpoints should be secure by default, typically captured in the application settings.</param>
    /// <param name="httpEndpointModel">The HTTP endpoint model.</param>
    /// <returns>Whether the provided <paramref name="element"/> was found to represent an endpoint.</returns>
    public static bool TryGetEndpoint(
        IElement element,
        string? defaultBasePath,
        bool securedByDefault,
        [NotNullWhen(true)] out IHttpEndpointModel? httpEndpointModel)
    {
        var securityModels = SecurityModelHelpers.GetSecurityModels(element).ToArray();

        var result = TryGetEndpoint(
            element: element,
            defaultBasePath: defaultBasePath,
            securedByDefault: securedByDefault,
            securityModels: securityModels,
            httpEndpoint: out var @out);

        httpEndpointModel = @out;
        return result;
    }

    private static HttpEndpointModel GetEndpoint(
        IElement element,
        string? defaultBasePath,
        bool securedByDefault,
        IReadOnlyCollection<ISecurityModel> securityModels)
    {
        if (!TryGetEndpoint(
                element,
                defaultBasePath: defaultBasePath,
                securedByDefault: securedByDefault,
                securityModels: securityModels,
                httpEndpoint: out HttpEndpointModel? model))
        {
            throw new Exception("Element is not an endpoint");
        }

        return model;
    }

    private static bool TryGetEndpoint(
        IElement element,
        string? defaultBasePath,
        bool securedByDefault,
        IReadOnlyCollection<ISecurityModel> securityModels,
        [NotNullWhen(true)] out HttpEndpointModel? httpEndpoint)
    {
        if (element.Stereotypes.Any(s => s.Name == "Http Settings (Obsolete)"))
        {
            throw new ElementException(element, "A migration is outstanding on the services designer. Please open the services designer which will run the migration automatically and be sure to press save regardless afterwards.");
        }

        if (!element.TryGetHttpSettings(out var httpSettings))
        {
            httpEndpoint = default;
            return false;
        }

        var hasUnsecured = element.TryGetUnsecured(out _);

        var baseRoute = GetBaseRoute(element, defaultBasePath);
        var subRoute = httpSettings!.Route;

        httpEndpoint = new HttpEndpointModel(
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
            securityModels: securityModels,
            securedByDefault: securedByDefault,
            allowAnonymous: hasUnsecured,
            internalElement: element,
            inputs: GetParameters(element, httpSettings).ToArray());
        return true;
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
            var version = apiVersion.ApplicableVersions.Max(x => x.Version)!;
            version = FormatVersion(version);
            routeConstruction = routeConstruction.Replace("{version}", version);
        }
        else if (element.ParentElement?.TryGetApiVersion(out var parentApiVersion) == true)
        {
            var version = parentApiVersion.ApplicableVersions.Max(x => x.Version)!;
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

    private static string GetBaseRoute(IElement element, string? defaultBasePath)
    {
        var baseRoute = element.ParentElement?.TryGetHttpServiceSettings(out var serviceSettings) == true &&
                        !string.IsNullOrWhiteSpace(serviceSettings!.Route)
            ? serviceSettings.Route
            : defaultBasePath;

        return baseRoute ?? string.Empty;
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