/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedDomainElement.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/command-azure-functions/command-on-changed/on-changed.ts
 */

(() => {
    const azureFunctionId = "7c1128f6-fdef-4bf9-8f15-acb54b5bfa89"; // from AzureFunctions (this) module
    const azureFunctionTriggerId = "a6411e1f-8199-4b18-b1a1-fd2aa73b1da6";
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"; // from WebApi module
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    const parameterSettingsId = "d01df110-1208-4af8-a913-92a49d219552"; // from WebApi module

    if (!element.hasStereotype(azureFunctionId)) {
        return;
    }

    if (element.getStereotype(azureFunctionId).getProperty(azureFunctionTriggerId).getValue() !== "Http Trigger") {
        element.removeStereotype(httpSettingsId);
        element.getChildren().forEach(x => x.removeStereotype(parameterSettingsId));
        return;
    }

    let httpSettings = element.getStereotype(httpSettingsId) ?? element.addStereotype(httpSettingsId);

    if (httpSettings.getProperty("Route").getValue()) {
        return;
    }

    const folderName = element.getParent().getName();
    const mappedEntityName = getMappedDomainElement(element)?.getName();

    const serviceRoute = toKebabCase(folderName);
    const serviceRouteIdentifier = element.getChildren().some(x => x.getName().toLowerCase() == `${singularize(folderName.toLowerCase())}id`)
        ? `/{${toCamelCase(singularize(folderName))}Id}`
        : element.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "";
    const subRoute = mappedEntityName && mappedEntityName != singularize(folderName) && serviceRouteIdentifier != `/{id}`
        ? `/${toKebabCase(pluralize(mappedEntityName))}${element.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : ""}`
        : getUnconventionalRoute() != "" ? `/${getUnconventionalRoute()}` : "";

    if (element.getName().startsWith("Create") || element.getName().startsWith("Add")) {
        httpSettings.getProperty("Verb").setValue("POST");
    } else if (element.getName().startsWith("Delete") || element.getName().startsWith("Remove")) {
        httpSettings.getProperty("Verb").setValue("DELETE");
    } else {
        httpSettings.getProperty("Verb").setValue("PUT");
    }

    httpSettings.getProperty("Route").setValue(`${serviceRoute}${serviceRouteIdentifier}${subRoute}`)

    if (element.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }

    function getUnconventionalRoute() {
        if ((element.getName().startsWith("Create") ||
            element.getName().startsWith("Update") ||
            element.getName().startsWith("Delete")) &&
            (serviceRouteIdentifier == "" || serviceRouteIdentifier == `/{id}`)) {
            return "";
        }
        let queryRoute = toKebabCase(removeSuffix(removePrefix(element.getName(), "Create", "Update", "Delete"), "Command"));
        return removeSuffix(queryRoute, toKebabCase(mappedEntityName ?? singularize(folderName)), serviceRoute, "-");
    }
})();
