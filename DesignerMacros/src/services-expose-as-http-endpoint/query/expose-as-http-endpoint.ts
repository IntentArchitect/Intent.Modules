/// <reference path="../common/on-expose-functions.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/query/expose-as-http-endpoint.ts
 */

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void{

    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    element.addStereotype(httpSettingsId);

    let httpSettings = element.getStereotype(httpSettingsId);

    let folderName = element.getParent().getName();
    let mappedEntity = element.getMapping()?.getElement()?.getName();

    let serviceRoute = getServiceRoute(element);
    let conventionIdAttribute = element.getChildren().find(x => x.getName().toLowerCase() == `${singularize(folderName.toLowerCase())}id`);
    let idAttribute = element.getChildren().find(x => x.getName().toLowerCase() == "id");
    let serviceRouteIdentifier = conventionIdAttribute 
        ? `/{${toCamelCase(conventionIdAttribute.getName()) }}`
        : idAttribute ? `/{${toCamelCase(idAttribute.getName())}}` : "";
    let subRoute = mappedEntity && mappedEntity != singularize(folderName) && serviceRouteIdentifier != `/{id}` 
        ? `/${toKebabCase(pluralize(mappedEntity))}${idAttribute ? `/{${toCamelCase(idAttribute.getName())}}` : ""}` 
        : getUnconventionalRoute(serviceRouteIdentifier, mappedEntity, folderName, serviceRoute) != "" ? `/${getUnconventionalRoute(serviceRouteIdentifier, mappedEntity, folderName, serviceRoute)}` : "";

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(`api/${serviceRoute}${serviceRouteIdentifier}${subRoute}`);
}


function getUnconventionalRoute(serviceRouteIdentifier : string, mappedEntity : string, folderName : string, serviceRoute : string) : string{
    if ((element.getName().startsWith("Get") ||
        element.getName().startsWith("Find") ||
        element.getName().startsWith("Lookup")) && 
        (serviceRouteIdentifier == `/{id}`)) {
        return "";
    }

    const withoutPrefixes = removePrefix(element.getName(), "Get", "Find", "Lookup");
    const withoutRequestSuffix = removeSuffix(withoutPrefixes, "Request");
    const withoutQuerySuffix = removeSuffix(withoutRequestSuffix, "Query");
    return removePrefix(toKebabCase(withoutQuerySuffix), serviceRoute, toKebabCase(mappedEntity ?? singularize(folderName)), "-");
}

exposeAsHttpEndPoint(element);