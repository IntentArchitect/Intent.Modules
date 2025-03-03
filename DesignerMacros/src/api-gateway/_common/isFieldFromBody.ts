/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function isFieldFromBody(downStreamOperation: MacroApi.Context.IElementApi , dtoField: MacroApi.Context.IElementApi) {
    if (dtoField.specialization != "DTO-Field") {
        throw Error(`Element "${dtoField.id}, ${dtoField.getName()}" is not of type DTO-Field.`);
    }
    let httpSpecifiedElement = downStreamOperation.specialization === "Operation"
        ? downStreamOperation.getChildren("Parameter").filter(param => param.typeReference?.getType().specialization === "DTO")[0]
        : downStreamOperation;
    let httpSettingsRoute = httpSpecifiedElement.getStereotype("Http Settings")?.getProperty("Route")?.getValue() as string;
    let paramSource = dtoField.getStereotype("Parameter Settings")?.getProperty("Source")?.getValue();
    if (!paramSource || paramSource == "Default") {
        return httpSettingsRoute.toLowerCase().indexOf(`{${dtoField.getName().toLowerCase()}}`) >= 0;
    }
    return paramSource == "Body";
}