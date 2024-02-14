/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function matchOperation(possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck: string, verbToCheck: string): boolean {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let apiVersionSettingId = "20855f03-c663-4ec6-b106-de06be98f1fe";//Api Version Setting

    let currentHttpSettings = possibleDuplicate.getStereotype(httpSettingsId);
    if (currentHttpSettings == null) {
        return false;
    }

    let result = routeToCheck == getOperationPath(possibleDuplicate) && verbToCheck == currentHttpSettings.getProperty("Verb").value;

    if (result && (changingElement.getStereotype(apiVersionSettingId) || changingElement.getParent().getStereotype(apiVersionSettingId))) {
        let changingVersions: string[];
        let duplicateVersions: string[];
        let changeStereoType: MacroApi.Context.IStereotypeApi;
        let possibleDuplicateStereoType: MacroApi.Context.IStereotypeApi;

        if (changingElement.getStereotype(apiVersionSettingId)) {
            changeStereoType = changingElement.getStereotype(apiVersionSettingId)
        } else if (changingElement.getParent().getStereotype(apiVersionSettingId)) {
            changeStereoType = changingElement.getParent().getStereotype(apiVersionSettingId);
        } else {
            return result;
        }

        if (possibleDuplicate.getStereotype(apiVersionSettingId)) {
            possibleDuplicateStereoType = possibleDuplicate.getStereotype(apiVersionSettingId);
        } else if (possibleDuplicate.getParent().getStereotype(apiVersionSettingId)) {
            possibleDuplicateStereoType = possibleDuplicate.getParent().getStereotype(apiVersionSettingId);
        } else {
            return result;
        }

        changingVersions = JSON.parse(changeStereoType.getProperty("Applicable Versions").value);
        duplicateVersions = JSON.parse(possibleDuplicateStereoType.getProperty("Applicable Versions").value);
        let intersection = changingVersions.filter(value => duplicateVersions.includes(value));
        return intersection.length > 0;
    }
    return result;
}
