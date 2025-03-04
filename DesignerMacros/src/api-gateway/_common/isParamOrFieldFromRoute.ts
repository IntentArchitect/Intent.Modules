/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function isParamOrFieldFromRoute(downStreamOperation: MacroApi.Context.IElementApi , paramOrField: MacroApi.Context.IElementApi): boolean {
    let httpSettingsRoute = downStreamOperation.getStereotype("Http Settings")?.getProperty("Route")?.getValue() as string;
    let paramSource = paramOrField.getStereotype("Parameter Settings")?.getProperty("Source")?.getValue();
    if (!paramSource || paramSource == "Default" || paramSource == "Route") {
        return httpSettingsRoute.toLowerCase().indexOf(`{${paramOrField.getName().toLowerCase()}}`) >= 0;
    }
    return false;
}