/// <reference path="../_common/getFolderParts.ts" />
/// <reference path="../_common/getRouteParts.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function exposeAsHttpEndPoint(command: MacroApi.Context.IElementApi): void {
    const mappedDetails = getMappedRequestDetails(command);

    // Add the folder parts
    const routeParts: string[] = ["api"];
    routeParts.push(...getFolderParts(command, mappedDetails));

    if (mappedDetails != null) {
        routeParts.push(...getRouteParts(command, mappedDetails));
    }
    else if (!["Create", "Update", "Delete"].some(x => command.getName().startsWith(x))) {
        routeParts.push(toKebabCase(removeSuffix(command.getName(), "Request", "Command")));
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    const httpSettings = command.getStereotype(httpSettingsId) ?? command.addStereotype(httpSettingsId);
    httpSettings.getProperty("Route").setValue(routeParts.join("/"))

    if (["Create", "Add"].some(x => command.getName().startsWith(x))) {
        httpSettings.getProperty("Verb").setValue("POST");
    } else if (["Delete", "Remove"].some(x => command.getName().startsWith(x))) {
        httpSettings.getProperty("Verb").setValue("DELETE");
    } else {
        httpSettings.getProperty("Verb").setValue("PUT");
    }

    if (command.typeReference.getType()?.specialization == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
}

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-as-http-endpoint/command/expose-as-http-endpoint.ts
 */
exposeAsHttpEndPoint(element);
