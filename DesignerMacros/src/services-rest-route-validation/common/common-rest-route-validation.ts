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

    //again need to resolve paths for this to work
    if (absoluteRouteToCheck.toLocaleLowerCase().includes("[action]")){
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

    return "";
}

function getOperationPath(operation : MacroApi.Context.IElementApi):string{
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";        
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let serviceHttpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";

    let httpSettings = operation.getStereotype(httpSettingsId);
    if (httpSettings == null){
        return "";
    }

    let operationPath = httpSettings.getProperty("Route").value;
    if (operation.getParent().getStereotype(serviceHttpServiceSettingsId)){
         let servicePath = operation.getParent().getStereotype(serviceHttpServiceSettingsId).getProperty("Route").value;
         return `${servicePath}/${operationPath}`;
    }
    return `[${operation.getParent().getName()}]/${operationPath}`;
}

function matchCommandOrQuery(possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string) : boolean{
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let apiVersionSettingId ="20855f03-c663-4ec6-b106-de06be98f1fe";//Api Version Setting

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null){
        return false;
    }

    let result = routeToCheck == currentHttpSettings.getProperty("Route").value && verbToCheck == currentHttpSettings.getProperty("Verb").value;
    if (result && changingElement.getStereotype(apiVersionSettingId)){
        let changingVersions = JSON.parse( changingElement.getStereotype(apiVersionSettingId).getProperty("Applicable Versions").value);
        let duplicateVersions = JSON.parse( possibleDuplicate.getStereotype(apiVersionSettingId).getProperty("Applicable Versions").value);
        let intersection = changingVersions.filter(value => duplicateVersions.includes(value));
        return intersection.length > 0;
    }
    return result;
}

function matchOperation(possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string) : boolean{
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let apiVersionSettingId ="20855f03-c663-4ec6-b106-de06be98f1fe";//Api Version Setting

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null){
        return false;
    }

    let result = routeToCheck == getOperationPath(possibleDuplicate) && verbToCheck == currentHttpSettings.getProperty("Verb").value;
    
    if (result && (changingElement.getStereotype(apiVersionSettingId) || changingElement.getParent().getStereotype(apiVersionSettingId) )){
        let changingVersions : string;
        let duplicateVersions: string;
        let changeStereoType : MacroApi.Context.IStereotypeApi;
        let possibleDuplicateStereoType : MacroApi.Context.IStereotypeApi;
        if (changingElement.getParent().getStereotype(apiVersionSettingId)) changeStereoType = changingElement.getParent().getStereotype(apiVersionSettingId);
        if (changingElement.getStereotype(apiVersionSettingId)) changeStereoType = changingElement.getStereotype(apiVersionSettingId);
        if (possibleDuplicate.getParent().getStereotype(apiVersionSettingId)) possibleDuplicateStereoType = possibleDuplicate.getParent().getStereotype(apiVersionSettingId);
        if (possibleDuplicate.getStereotype(apiVersionSettingId)) possibleDuplicateStereoType = possibleDuplicate.getStereotype(apiVersionSettingId);

        changingVersions = JSON.parse( changeStereoType.getProperty("Applicable Versions").value);
        duplicateVersions = JSON.parse( possibleDuplicateStereoType.getProperty("Applicable Versions").value);
        let intersection = changingVersions.filter(value => duplicateVersions.includes(value));
        return intersection.length > 0;
    }
    return result;
}

function checkForDuplicates(elements : MacroApi.Context.IElementApi[]
    , element : MacroApi.Context.IElementApi
    , routeToCheck : string
    , verbToCheck : string
    , match: (possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck : string, verbToCheck : string ) => boolean)  : string | null {

    let duplicate : MacroApi.Context.IElementApi | undefined;    
    elements.forEach(e => {
        if (duplicate != null)
            return;
        if (e.id == element.id)
            return;
        if (match(e, element, routeToCheck, verbToCheck)){
            duplicate = e;
        }
    });
    if (duplicate){
        return `Duplicate rest route ${element.getName()}(${element.specialization}) with ${duplicate.getName()}(${duplicate.specialization}) - ${routeToCheck}`;
    }
    return null;
}
