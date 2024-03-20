/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedDomainElement.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/azure-functions/query-on-changed/query-on-changed.ts
 */

(() => {
    const azureFunctionId = "7c1128f6-fdef-4bf9-8f15-acb54b5bfa89"; // from AzureFunctions (this) module
    const azureFunctionTriggerId = "a6411e1f-8199-4b18-b1a1-fd2aa73b1da6";
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"; // from WebApi module
    const parameterSettingsId = "d01df110-1208-4af8-a913-92a49d219552"; // from WebApi module

    if (element.getStereotype(azureFunctionId)?.getProperty(azureFunctionTriggerId).getValue() !== "Http Trigger") {
        element.removeStereotype(httpSettingsId);
        element.getChildren().forEach(x => x.removeStereotype(parameterSettingsId));
        return;
    }

    const httpSettings = element.getStereotype(httpSettingsId) ?? element.addStereotype(httpSettingsId);

    const folderName = element.getParent().getName();
    const mappedEntityName = getMappedDomainElement(element)?.getName();

    if (httpSettings.getProperty("Route").getValue()) {
        return;
    }

    const serviceRoute = toKebabCase(folderName);
    const serviceRouteIdentifier = element.getChildren().some(x => x.getName().toLowerCase() == `${singularize(folderName.toLowerCase())}id`)
        ? `/{${toCamelCase(singularize(folderName))}Id}`
        : element.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "";
    const subRoute = mappedEntityName && mappedEntityName != singularize(folderName) && serviceRouteIdentifier != `/{id}`
        ? `/${toKebabCase(pluralize(mappedEntityName))}${element.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : ""}`
        : getUnconventionalRoute() != "" ? `/${getUnconventionalRoute()}` : "";

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(`${serviceRoute}${serviceRouteIdentifier}${subRoute}`);

    function getUnconventionalRoute() {
        if ((element.getName().startsWith("Get") ||
            element.getName().startsWith("Find") ||
            element.getName().startsWith("Lookup")) &&
            (serviceRouteIdentifier == `/{id}`)) {
            return "";
        }
        const queryRoute = toKebabCase(removeSuffix(removePrefix(element.getName(), "Get", "Find", "Lookup"), "Query"));
        return removePrefix(queryRoute, serviceRoute, toKebabCase(mappedEntityName ?? singularize(folderName)), "-");
    }
})();
