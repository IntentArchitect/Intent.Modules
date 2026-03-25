/// <reference path="../_common/common.ts" />
/// <reference path="../../common/getMappedDomainElement.ts" />

function exposeCommandAsHttpEndPoint(command: MacroApi.Context.IElementApi): void {
    let httpSettings = _prepareCommandAsHttpEndpoint(command);

    if (["Create", "Add"].some(x => command.getName().startsWith(x))) {
        httpSettings.getProperty("Verb").setValue("POST");
    } else if (["Delete", "Remove"].some(x => command.getName().startsWith(x))) {
        httpSettings.getProperty("Verb").setValue("DELETE");
    } else if (["Patch"].some(x => command.getName().startsWith(x))) {
        httpSettings.getProperty("Verb").setValue("PATCH");
    } else {
        httpSettings.getProperty("Verb").setValue("PUT");
    }
}

function exposeCommandAsHttpPatchEndpoint(command: MacroApi.Context.IElementApi): void {
    let httpSettings = _prepareCommandAsHttpEndpoint(command);
    httpSettings.getProperty("Verb").setValue("PATCH");
}

function _prepareCommandAsHttpEndpoint(command: MacroApi.Context.IElementApi): MacroApi.Context.IStereotypeApi {
    const domainElement = getMappedDomainElement(command);

    // Add the folder parts
    const routeParts: string[] = [];
    const defaultRoutePrefix = getDefaultRoutePrefix(false);
    const defaultRoutePrefixParts = (!defaultRoutePrefix || defaultRoutePrefix == "") ? [] : defaultRoutePrefix.split("/");
    if (defaultRoutePrefixParts?.length > 0) {
        routeParts.push(...defaultRoutePrefixParts);
    }

    let folderParts = getFolderParts(command, domainElement);
    routeParts.push(...folderParts);

    if (domainElement != null) {
        routeParts.push(...getRouteParts(command, domainElement));
    }
    else {
        routeParts.push(...generateNonDefaultEndpointRouteName(command, ``, folderParts));
    }

    let endpointInputIdElement = command.getChildren().filter(x => x.hasMetadata("endpoint-input-id"))[0];
    if (endpointInputIdElement) {
        routeParts.push(`{${toCamelCase(endpointInputIdElement.getName())}}`);
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    const httpSettings = command.getStereotype(httpSettingsId) ?? command.addStereotype(httpSettingsId);
    httpSettings.getProperty("Route").setValue(routeParts.join("/"))

    if (command.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
    return httpSettings;
}