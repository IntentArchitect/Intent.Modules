/// <reference path="../_common/getFolderParts.ts" />
/// <reference path="../_common/getRouteParts.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="../_common/getDefaultRoutePrefix.ts" />
/// <reference path="../../common/getMappedEntity.ts" />

function exposeAsHttpEndPoint(request: MacroApi.Context.IElementApi): void {
    const { entity, owningEntity } = getMappedEntity(request);

    // Add the folder parts
    const routeParts: string[] = [];
    const defaultRoutePrefixParts = getDefaultRoutePrefix(false).split("/");
    routeParts.push(...defaultRoutePrefixParts);
    routeParts.push(...getFolderParts(request, entity, owningEntity));

    if (entity != null) {
        routeParts.push(...getRouteParts(request, entity, owningEntity));
    }
    else if (!["Get", "Find", "Lookup"].some(x => request.getName().startsWith(x))) {
        routeParts.push(toKebabCase(removeSuffix(request.getName(), "Request", "Query")));
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettings = request.getStereotype(httpSettingsId) ?? request.addStereotype(httpSettingsId);

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(routeParts.join("/"))
}

