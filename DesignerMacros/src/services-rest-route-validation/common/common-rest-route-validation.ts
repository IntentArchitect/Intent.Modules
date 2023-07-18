/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function validateRestRoutes(element: MacroApi.Context.IElementApi): String{   
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettings = element.getStereotype(httpSettingsId);
    if (httpSettings == null){
        return "";
    }
    let myId = element.id;
    let commands = lookupTypesOf("Command");
    let duplicate = FindDuplicate(commands, myId, httpSettings);
    if (duplicate){
        let myRoute = httpSettings.getProperty("Route").getValue();
        return `Duplicate rest route ${element.getName()} with ${duplicate.getName()} - ${myRoute}`;
    }
    let queries = lookupTypesOf("Query");
    duplicate = FindDuplicate(queries, myId, httpSettings);
    if (duplicate){
        let myRoute = httpSettings.getProperty("Route").getValue();
        return `Duplicate rest route ${element.getName()} with ${duplicate.getName()} - ${myRoute}`;
    }
    return "";
}

function FindDuplicate(elements : MacroApi.Context.IElementApi[], elementId : string, httpSettings : MacroApi.Context.IStereotypeApi) : MacroApi.Context.IElementApi {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let routeToCheck = httpSettings.getProperty("Route").getValue();
    let verbToCheck = httpSettings.getProperty("Verb").getValue();
    let result = null;
    elements.forEach(e => {
        if (result != null)
            return;
        if (e.id == elementId)
            return;
        let currentHttpSettings = e.getStereotype(httpSettingsId);
        if (currentHttpSettings == null){
            return;
        }
        if (routeToCheck == currentHttpSettings.getProperty("Route").getValue() && verbToCheck == currentHttpSettings.getProperty("Verb").getValue()){
            result = e;
        }
    });
    return result;
}
