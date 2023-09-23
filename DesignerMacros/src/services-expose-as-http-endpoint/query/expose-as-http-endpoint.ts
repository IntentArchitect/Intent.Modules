/// <reference path="../_common/getFolderParts.ts" />
/// <reference path="../_common/getRouteParts.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="../_common/getDefaultRoutePrefix.ts" />

function exposeAsHttpEndPoint(query: MacroApi.Context.IElementApi): void {
    const mappedDetails = getMappedRequestDetails(query);

    // Add the folder parts
    const routeParts: string[] = [];
    const defaultRoutePrefixParts = getDefaultRoutePrefix(false).split("/");
    routeParts.push(...defaultRoutePrefixParts);
    routeParts.push(...getFolderParts(query, mappedDetails));

    if (mappedDetails != null) {
        routeParts.push(...getRouteParts(query, mappedDetails));
    }
    else if (!["Get", "Find", "Lookup"].some(x => query.getName().startsWith(x))) {
        routeParts.push(toKebabCase(removeSuffix(query.getName(), "Request", "Query")));
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettings = query.getStereotype(httpSettingsId) ?? query.addStereotype(httpSettingsId);

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(routeParts.join("/"))
}

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/query/expose-as-http-endpoint.ts
 */
exposeAsHttpEndPoint(element);