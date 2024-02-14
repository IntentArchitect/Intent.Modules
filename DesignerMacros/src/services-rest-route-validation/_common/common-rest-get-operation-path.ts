/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function getOperationPath(operation: MacroApi.Context.IElementApi): string {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let serviceHttpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";

    let httpSettings = operation.getStereotype(httpSettingsId);
    if (httpSettings == null) {
        return "";
    }

    let operationPath = httpSettings.getProperty("Route").value;
    if (operation.getParent().getStereotype(serviceHttpServiceSettingsId) && operation.getParent().getStereotype(serviceHttpServiceSettingsId).getProperty("Route").value != "") {
        let servicePath = operation.getParent().getStereotype(serviceHttpServiceSettingsId).getProperty("Route").value;
        if (servicePath.toLocaleLowerCase().includes('[controller]')) {
            servicePath = servicePath.replace(/\[controller\]/gi, `[${operation.getParent().getName()}]`);
        }
        return `${servicePath}/${operationPath}`;
    }
    //We don't know how the service name will be transformed so we add [{ServiceName}] to represent the transform
    return `[${operation.getParent().getName()}]/${operationPath}`;
}