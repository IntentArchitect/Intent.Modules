/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function validateRestRoutes(element: MacroApi.Context.IElementApi): String{   

    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettings = element.getStereotype(httpSettingsId);
    if (httpSettings == null){
        return "";
    }

    let absoluteRouteToCheck = httpSettings.getProperty("Route").value;
    if (element.specialization == "Operation"){
        absoluteRouteToCheck = getOperationPath(element);
    }
    let verbToCheck = httpSettings.getProperty("Verb").value;

    let commands = lookupTypesOf("Command");
    let message = checkForDuplicates(commands, element, absoluteRouteToCheck, verbToCheck, matchCommandOrQuery);
    if (message != null) return message;

    let queries = lookupTypesOf("Query");
    message = checkForDuplicates(queries, element, absoluteRouteToCheck, verbToCheck, matchCommandOrQuery);
    if (message != null) return message;

    let operations = lookupTypesOf("Operation");
    message = checkForDuplicates(operations, element, absoluteRouteToCheck, verbToCheck, matchOperation);
    if (message != null) return message;

    return "";
}

function getOperationPath(operation : MacroApi.Context.IElementApi):string{
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";        
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let httpSettings = operation.getStereotype(httpSettingsId);
    if (httpSettings == null){
        return "";
    }

    let operationPath = httpSettings.getProperty("Route").getValue();
    let servicePath = operation.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").value;
    return `${servicePath}/${operationPath}`;
}

function matchCommandOrQuery(possibleDuplicate: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string) : boolean{
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null){
        return false;
    }

    return routeToCheck == currentHttpSettings.getProperty("Route").value && verbToCheck == currentHttpSettings.getProperty("Verb").value;
}

function matchOperation(possibleDuplicate: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string) : boolean{
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null){
        return false;
    }

    return routeToCheck == getOperationPath(possibleDuplicate) && verbToCheck == currentHttpSettings.getProperty("Verb").value;
}

function checkForDuplicates(elements : MacroApi.Context.IElementApi[]
    , element : MacroApi.Context.IElementApi
    , routeToCheck : string
    , verbToCheck : string
    , match: (possibleDuplicate: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string ) => boolean)  : String {

    let duplicate = null;
    elements.forEach(e => {
        if (duplicate != null)
            return;
        if (e.id == element.id)
            return;
        if (match(e, routeToCheck, verbToCheck)){
            duplicate = e;
        }
    });
    if (duplicate){
        return `Duplicate rest route ${element.getName()}(${element.specialization}) with ${duplicate.getName()}(${duplicate.specialization}) - ${routeToCheck}`;
    }
    return null;
}
