/// <reference path="common.ts" />

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module

    if (!element.getParent().hasStereotype(httpServiceSettingsId)) {
        element.getParent().addStereotype(httpServiceSettingsId);

        let serviceBaseName = removeSuffix(element.getParent().getName(), "Service");
        element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").setValue(getRoute(serviceBaseName));
    }

    let parentRoute = element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").getValue().toString();

    applyHttpSettingsToOperations(element, parentRoute);
}

function getRoute(serviceBaseName: string): string {
    return `${getDefaultRoutePrefix(true)}${kebabCaseAcronymCorrection(toKebabCase(serviceBaseName), serviceBaseName)}`;
}

