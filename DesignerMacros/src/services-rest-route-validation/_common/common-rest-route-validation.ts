/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function validateRestRoutes(element: MacroApi.Context.IElementApi): String {

    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let httpSettings = element.getStereotype(httpSettingsId);
    if (httpSettings == null) {
        return "";
    }

    let absoluteRouteToCheck = httpSettings.getProperty("Route").value;
    if (element.specialization == "Operation") {
        absoluteRouteToCheck = getOperationPath(element);
    }

    //again need to resolve paths for this to work
    if (absoluteRouteToCheck.toLocaleLowerCase().includes("[action]")) {
        return "";
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

    message = matchParameters(element, absoluteRouteToCheck, verbToCheck);
    if (message != null) return message;

    return "";
}
