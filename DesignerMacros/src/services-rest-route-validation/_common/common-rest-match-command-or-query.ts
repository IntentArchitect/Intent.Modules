/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function matchCommandOrQuery(possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck: string, verbToCheck: string): boolean {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let apiVersionSettingId = "20855f03-c663-4ec6-b106-de06be98f1fe";//Api Version Setting

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null) {
        return false;
    }

    let result = routeToCheck == currentHttpSettings.getProperty("Route").value && verbToCheck == currentHttpSettings.getProperty("Verb").value;
    if (result && changingElement.getStereotype(apiVersionSettingId)) {
        let changingVersions = JSON.parse(changingElement.getStereotype(apiVersionSettingId).getProperty("Applicable Versions").value);
        let duplicateVersions = JSON.parse(possibleDuplicate.getStereotype(apiVersionSettingId).getProperty("Applicable Versions").value);
        let intersection = changingVersions.filter(value => duplicateVersions.includes(value));
        return intersection.length > 0;
    }
    return result;
}
