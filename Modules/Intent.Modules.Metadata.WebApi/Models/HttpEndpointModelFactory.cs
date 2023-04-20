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
        if (element.Stereotypes.Any(s => s.Name == "Http Settings (Obsolete)"))
        {
            throw new Exception("A migration is outstanding on the services designer. Please open the services designer which will run the migration automatically and be sure to press save regardless afterwards.");
        }

        if (!element.TryGetHttpSettings(out var httpSettings))
        {
            return null;
        }

        var hasSecured = element.TryGetSecured(out _);
        var hasUnsecured = element.TryGetSecured(out _);
        var hasAuthorize = element.TryGetAuthorize(out _);
        if ((hasSecured || hasAuthorize) && hasUnsecured)
        {
            throw new Exception($"{element.Name} [{element.Id}] cannot require authorization and allow-anonymous at the same time");
        }

        return new HttpEndpointModel(
            name: element.SpecializationTypeId switch
            {
                Constants.ElementTypeIds.Command => element.Name.RemoveSuffix("Command"),
                Constants.ElementTypeIds.Query => element.Name.RemoveSuffix("Query"),
                _ => element.Name
            },
            verb: httpSettings.Verb ?? element.SpecializationTypeId switch
            {
                Constants.ElementTypeIds.Operation => HttpVerb.Post,
                Constants.ElementTypeIds.Command => HttpVerb.Post,
                Constants.ElementTypeIds.Query => HttpVerb.Get,
                _ => throw new InvalidOperationException($"Unknown type: \"{element.SpecializationType}\" ({element.SpecializationTypeId})")
            },
            baseRoute: GetBaseRoute(element),
            subRoute: httpSettings.Route,
            mediaType: httpSettings.ReturnTypeMediatype,
            requiresAuthorization: hasSecured || hasAuthorize,
            allowAnonymous: hasUnsecured,
            internalElement: element,
            inputs: GetParameters(element, httpSettings)
                .ToArray());
    }

    private static string? GetBaseRoute(IElement element)
    {
        var baseRoute = element.ParentElement?.TryGetHttpServiceSettings(out var serviceSettings) == true &&
                        !string.IsNullOrWhiteSpace(serviceSettings.Route)
            ? serviceSettings.Route
            : null;

        // At present this is hardcoded to accommodate the default for C# and wouldn't work for
        // other techs like Java which has a fallback convention of kebab-casing the element
        // name. The ideal fix is to change the Services designer to auto apply a default of
        // $"api/{service.Name}" to the stereotype's Route property. This improvement to the
        // designer is being deferred until we start making proxies for other languages.
        if (baseRoute == null &&
            element.ParentElement?.SpecializationTypeId == Constants.ElementTypeIds.Service)
        {
            baseRoute = "api/[controller]";
        }

        // It's a smell that we're applying a C# WebApi convention for this which is intended
        // to be consumed by any kind of technology, but unlikely to cause an issue since
        // [controller] convention doesn't seem to be used elsewhere that we're aware of.
        if (element.ParentElement != null)
        {
            baseRoute = baseRoute?.Replace("[controller]", element.ParentElement.Name.RemoveSuffix("Controller", "Service"));
        }

        return baseRoute;
    }

    private static IEnumerable<IHttpEndpointInputModel> GetParameters(
        IElement element,
        HttpSettings httpSettings)
    {
        var isForCqrs =
            element.SpecializationTypeId is Constants.ElementTypeIds.Query or Constants.ElementTypeIds.Command;
        var hasNonRouteParameter = false;

        foreach (var childElement in element.ChildElements)
        {
            var hasParameterSettings = !childElement.TryGetParameterSettings(out var parameterSettings);
            var routeContainsParameter = httpSettings.Route?.Contains($"{{{childElement.Name.ToCamelCase()}}}") == true;

            if (isForCqrs && !hasParameterSettings && !routeContainsParameter)
            {
                hasNonRouteParameter = true;
                continue;
            }

            yield return new HttpEndpointInputModel(
                id: childElement.Id,
                name: childElement.Name.ToCamelCase(),
                typeReference: childElement.TypeReference,
                source: parameterSettings.Source ?? GetSource(element, childElement),
                headerName: parameterSettings.HeaderName,
                mappedPayloadProperty: childElement);
        }

        if (isForCqrs && hasNonRouteParameter)
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
                mappedPayloadProperty: null);
        }
    }

    private static HttpInputSource? GetSource(IElement element, IElement childElement)
    {
        element.TryGetHttpSettings(out var httpSettings);

        if (childElement.TypeReference.Element.IsTypeDefinitionModel() &&
            httpSettings.Route?.Contains($"{{{childElement.Name}}}") == true)
        {
            return HttpInputSource.FromRoute;
        }

        if (httpSettings.Verb is HttpVerb.Get or HttpVerb.Delete &&
            !childElement.TypeReference.Element.IsTypeDefinitionModel())
        {
            return HttpInputSource.FromQuery;
        }

        if (httpSettings.Verb is HttpVerb.Post or HttpVerb.Put &&
            !childElement.TypeReference.Element.IsTypeDefinitionModel())
        {
            return HttpInputSource.FromBody;
        }

        return null;
    }
}