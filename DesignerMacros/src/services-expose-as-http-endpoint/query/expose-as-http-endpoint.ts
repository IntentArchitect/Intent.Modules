/// <reference path="../_common/on-expose-functions.ts" />

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    element.addStereotype(httpSettingsId);

    const httpSettings = element.getStereotype(httpSettingsId);

    const folderName = element.getParent().getName();
    const mappedEntity = element.getMapping()?.getElement();

    const serviceRoute = getServiceRoute(element);
    const primaryDomainEntity = getDomainEntity(folderName);
    const serviceRouteIdentifier = getServiceRouteIdentifier(element, primaryDomainEntity, folderName);

    let subRoute = "";
    if (mappedEntity && singularize(mappedEntity.getName()) != singularize(folderName) && serviceRouteIdentifier != `/{id}`) {
        subRoute = getConventionalSubRoute(element, mappedEntity);
    }
    else {
        const optionalSubRoute = getUnconventionalRoute(serviceRouteIdentifier, mappedEntity?.getName(), folderName);
        if (optionalSubRoute != "") {
            subRoute = `/${optionalSubRoute}`;
        }
    }

    httpSettings.getProperty("Verb").setValue("GET");
    httpSettings.getProperty("Route").setValue(`api/${serviceRoute}${serviceRouteIdentifier}${subRoute}`);
}

function getUnconventionalRoute(serviceRouteIdentifier: string, mappedEntityName: string, folderName: string): string {
    if ((element.getName().startsWith("Get") ||
        element.getName().startsWith("Find") ||
        element.getName().startsWith("Lookup")) &&
        (serviceRouteIdentifier == `/{id}`)
    ) {
        return "";
    }

    const withoutPrefixes = removePrefix(element.getName(), "Get", "Find", "Lookup");
    const withoutRequestSuffix = removeSuffix(withoutPrefixes, "Request");
    const withoutQuerySuffix = removeSuffix(withoutRequestSuffix, "Query");

    return removePrefix(toKebabCase(withoutQuerySuffix), toKebabCase(folderName), toKebabCase(mappedEntityName ?? singularize(folderName)), "-");
}

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/query/expose-as-http-endpoint.ts
 */
exposeAsHttpEndPoint(element);