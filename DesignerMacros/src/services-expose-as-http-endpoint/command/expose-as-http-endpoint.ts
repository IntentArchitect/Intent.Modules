/// <reference path="../common/on-expose-functions.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/command/expose-as-http-endpoint.ts
 */


function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void{
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    element.addStereotype(httpSettingsId);

    let httpSettings = element.getStereotype(httpSettingsId);

    let folderName = element.getParent().getName();
    let mappedEntity = element.getMapping()?.getPath()[0]?.getElement()?.getName();

    let serviceRoute = getServiceRoute(element);
    let conventionIdAttribute = element.getChildren().find(x => x.getName().toLowerCase() == `${singularize(folderName.toLowerCase())}id`);
    let idAttribute = element.getChildren().find(x => x.getName().toLowerCase() == "id");
    let serviceRouteIdentifier = conventionIdAttribute 
        ? `/{${toCamelCase(conventionIdAttribute.getName()) }}`
        : idAttribute ? `/{${toCamelCase(idAttribute.getName())}}` : "";
    let subRoute = mappedEntity && mappedEntity != singularize(folderName) && serviceRouteIdentifier != `/{id}` 
        ? `/${toKebabCase(pluralize(mappedEntity))}${idAttribute ? `/{${toCamelCase(idAttribute.getName())}}` : ""}` 
        : getUnconventionalRoute(serviceRouteIdentifier, mappedEntity, folderName, serviceRoute) != "" ? `/${getUnconventionalRoute(serviceRouteIdentifier, mappedEntity, folderName, serviceRoute)}` : "";

    if (element.getName().startsWith("Create") || element.getName().startsWith("Add")) {
        httpSettings.getProperty("Verb").setValue("POST");
    } else if (element.getName().startsWith("Delete") || element.getName().startsWith("Remove")) {
        httpSettings.getProperty("Verb").setValue("DELETE");
    } else {
        httpSettings.getProperty("Verb").setValue("PUT");
    }

    httpSettings.getProperty("Route").setValue(`api/${serviceRoute}${serviceRouteIdentifier}${subRoute}`)

    if (element.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
}

function getUnconventionalRoute(serviceRouteIdentifier : string, mappedEntity : string, folderName : string, serviceRoute : string) : string {
    if ((element.getName().startsWith("Create") ||
        element.getName().startsWith("Update") ||
        element.getName().startsWith("Delete")) && 
        (serviceRouteIdentifier == "" || serviceRouteIdentifier == `/{id}`)) {
        return "";
    }

    const withoutPrefixes = removePrefix(element.getName(), "Create", "Update", "Delete");
    const withoutRequestSuffix = removeSuffix(withoutPrefixes, "Request");
    const withoutCommandSuffix = removeSuffix(withoutRequestSuffix, "Command");
    return removeSuffix(toKebabCase(withoutCommandSuffix), toKebabCase(mappedEntity ?? singularize(folderName)), serviceRoute, "-");
}

exposeAsHttpEndPoint(element);