/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function getOperationPath(operation: MacroApi.Context.IElementApi, shouldReplaceRouteParams: boolean): string {
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const serviceHttpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";

    let httpSettings = operation.getStereotype(httpSettingsId);
    if (httpSettings == null) {
        return "";
    }

    let counter = 0;
    let operationPath = replaceRouteParameters(httpSettings.getProperty("Route").value, shouldReplaceRouteParams);
    if (operation.getParent().getStereotype(serviceHttpServiceSettingsId) && operation.getParent().getStereotype(serviceHttpServiceSettingsId).getProperty("Route").value != "") {
        let servicePath = operation.getParent().getStereotype(serviceHttpServiceSettingsId).getProperty("Route").value;
        servicePath = replaceRouteParameters(servicePath, shouldReplaceRouteParams);
        if (servicePath.toLocaleLowerCase().includes('[controller]')) {
            servicePath = servicePath.replace(/\[controller\]/gi, `[${operation.getParent().getName()}]`);
        }
        return `${servicePath}/${operationPath}`;
    }
    //We don't know how the service name will be transformed so we add [{ServiceName}] to represent the transform
    return `[${operation.getParent().getName()}]/${operationPath}`;

    function replaceRouteParameters(route: string, enabled: boolean) : string {
        if (!enabled) { return route; }
        return route.replace(/\{([^}]+)\}/g, function(match, g1) { return ""+(counter++); });
    }
}