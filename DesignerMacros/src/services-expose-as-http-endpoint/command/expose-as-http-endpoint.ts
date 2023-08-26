/// <reference path="../_common/on-expose-functions.ts" />

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    element.addStereotype(httpSettingsId);

    let httpSettings = element.getStereotype(httpSettingsId);

    let folderName = element.getParent().getName();
    let mappedEntity = element.getMapping()?.getElement();

    let serviceRoute = getServiceRoute(element);
    let primaryDomainEntity = getDomainEntity(folderName);
    let serviceRouteIdentifier = getServiceRouteIdentifier(element, primaryDomainEntity, folderName);

    let subRoute = "";
    if (mappedEntity != null && mappedEntity.specialization === "Operation") {
        subRoute = `/${getOperationRoute(serviceRouteIdentifier, mappedEntity?.getName(), folderName, "Create", "Update", "Delete", "Add", "Remove")}`;
    } else if (mappedEntity != null && mappedEntity.specialization === "Class Constructor") {
        // no sub-route
    } else if (mappedEntity && singularize(mappedEntity.getName()) != singularize(folderName) && serviceRouteIdentifier != `/{id}`) {
        subRoute = getConventionalSubRoute(element, mappedEntity);
    } else {
        let optionalSubRoute = getUnconventionalRoute(serviceRouteIdentifier, mappedEntity?.getName(), folderName);
        if (optionalSubRoute != "") {
            subRoute = `/${optionalSubRoute}`;
        }
    }

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

function getUnconventionalRoute(serviceRouteIdentifier: string, mappedEntityName: string, folderName: string): string {
    if ((element.getName().startsWith("Create") ||
        element.getName().startsWith("Update") ||
        element.getName().startsWith("Delete")) &&
        (serviceRouteIdentifier == "" || serviceRouteIdentifier == `/{id}`)) {
        return "";
    }

    const withoutPrefixes = removePrefix(element.getName(), "Create", "Update", "Delete");
    const withoutRequestSuffix = removeSuffix(withoutPrefixes, "Request");
    const withoutCommandSuffix = removeSuffix(withoutRequestSuffix, "Command");
    return removeSuffix(toKebabCase(withoutCommandSuffix), toKebabCase(mappedEntityName ?? singularize(folderName)), toKebabCase(folderName), "-");
}

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/command/expose-as-http-endpoint.ts
 */
exposeAsHttpEndPoint(element);