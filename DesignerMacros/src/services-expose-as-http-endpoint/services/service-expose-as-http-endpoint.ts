/// <reference path="../_common/common.ts" />

function exposeServiceAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    const httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module

    if (!element.hasStereotype(httpServiceSettingsId)) {
        element.addStereotype(httpServiceSettingsId);

        let serviceBaseName = removeSuffix(element.getName(), "Service");
        element.getStereotype(httpServiceSettingsId).getProperty("Route").setValue(_getServiceRoute(serviceBaseName));
    }

    let parentRoute = element.getStereotype(httpServiceSettingsId).getProperty("Route").getValue().toString();

    element.getChildren("Operation").forEach(x => {
        applyHttpSettingsToOperations(x, parentRoute);
    })
}

function _getServiceRoute(serviceBaseName: string): string {
    return `${getDefaultRoutePrefix(true)}${toKebabCase(serviceBaseName)}`;
}