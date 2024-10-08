/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/getDefaultRoutePrefix.ts" />

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    const httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module

    if (!element.hasStereotype(httpServiceSettingsId)) {
        element.addStereotype(httpServiceSettingsId);

        let serviceBaseName = removeSuffix(element.getName(), "Service");
        element.getStereotype(httpServiceSettingsId).getProperty("Route").setValue(getRoute(serviceBaseName));
    }

    element.getChildren("Operation").forEach(x => {
        applyHttpSettingsToOperations(x);
    })
}

function getRoute(serviceBaseName: string): string {
    return `${getDefaultRoutePrefix(true)}${toKebabCase(serviceBaseName)}`;
}

function applyHttpSettingsToOperations(operation: MacroApi.Context.IElementApi): void {
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"; // from WebApi module
    const parameterSettingsId = "d01df110-1208-4af8-a913-92a49d219552"; // from WebApi module
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";

    if (!operation.hasStereotype(httpSettingsId)) {
        operation.addStereotype(httpSettingsId);
    }
    let httpSettings = operation.getStereotype(httpSettingsId);
    if (operation.getName().startsWith("Create")) {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(``)
    } else if (operation.getName().startsWith("Update")) {
        httpSettings.getProperty("Verb").setValue("PUT");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName().startsWith("Delete")) {
        httpSettings.getProperty("Verb").setValue("DELETE");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName().startsWith("Get") || operation.getName().startsWith("Find") || operation.getName().startsWith("Lookup")) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.typeReference.getType() != null) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${toKebabCase(operation.getName())}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`)
    } else {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(`${toKebabCase(operation.getName())}`)
    }

    operation.getChildren("Parameter").forEach(parameter => {
        if (!parameter.hasStereotype(parameterSettingsId)) {
            parameter.addStereotype(parameterSettingsId);
        }
    });

    if (operation.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
}


/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/service/expose-as-http-endpoint.ts
 */
exposeAsHttpEndPoint(element);