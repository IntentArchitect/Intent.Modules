/// <reference path="../_common/common.ts" />

function exposeOperationAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    let parentRoute = _getParentRoute(element);

    applyHttpSettingsToOperations(element, parentRoute);
}

function exposeOperationAsHttpPatchEndpoint(element: MacroApi.Context.IElementApi): void {
    let parentRoute = _getParentRoute(element);

    applyHttpSettingsToOperations(element, parentRoute);

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    element.getStereotype(httpSettingsId).getProperty("Verb").setValue("PATCH");
}

function _getParentRoute(element: MacroApi.Context.IElementApi) {
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module

    if (!element.getParent().hasStereotype(httpServiceSettingsId)) {
        element.getParent().addStereotype(httpServiceSettingsId);

        let serviceBaseName = removeSuffix(element.getParent().getName(), "Service");
        element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").setValue(_getOperationRoute(serviceBaseName));
    }

    let parentRoute = element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").getValue().toString();
    return parentRoute;
}

function _getOperationRoute(serviceBaseName: string): string {
    return `${getDefaultRoutePrefix(true)}${kebabCaseAcronymCorrection(toKebabCase(serviceBaseName), serviceBaseName)}`;
}
