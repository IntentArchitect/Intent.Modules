/// <reference path="../_common/getFolderParts.ts" />
/// <reference path="../_common/getRouteParts.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="../_common/getDefaultRoutePrefix.ts" />
/// <reference path="../../common/getMappedDomainElement.ts" />

function exposeAsHttpEndPoint(request: MacroApi.Context.IElementApi): void {
    const domainElement = getMappedDomainElement(request);

    // Add the folder parts
    const routeParts: string[] = [];
    const defaultRoutePrefixParts = getDefaultRoutePrefix(false).split("/");
    routeParts.push(...defaultRoutePrefixParts);

    let folderParts = getFolderParts(request, domainElement);
    routeParts.push(...folderParts);

    if (domainElement != null) {
        routeParts.push(...getRouteParts(request, domainElement));
    }
    else { 
        routeParts.push(...generateNonDefaultEndpointRouteName(request, ``, folderParts));
    }
    
    let endpointInputIdElement = request.getChildren().filter(x => x.hasMetadata("endpoint-input-id"))[0];
    if (endpointInputIdElement) {
        routeParts.push(`{${toCamelCase(endpointInputIdElement.getName())}}`);
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettings = request.getStereotype(httpSettingsId) ?? request.addStereotype(httpSettingsId);

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(routeParts.join("/"))

    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";

    if (request.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
}

