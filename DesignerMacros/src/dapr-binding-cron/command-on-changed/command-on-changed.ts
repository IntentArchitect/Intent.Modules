/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Binding.Cron
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/dapr-binding-cron/command-on-changed/command-on-changed.ts
 */

async function execute(element: MacroApi.Context.IElementApi): Promise<void> {
    const daprCronBinding = element.getStereotype("48db7f06-3edd-488a-8838-1f7f33b81eb0");
    if (daprCronBinding == null) {
        return;
    }

    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettings = element.getStereotype(httpSettingsId);
    if (httpSettings == null) {
        element.addStereotype(httpSettingsId);
        httpSettings = element.getStereotype(httpSettingsId);
        httpSettings.getProperty("Route").setValue(toKebabCase(removeSuffix(element.getName(), "Command")));
    }

    if (httpSettings.getProperty("Verb").getValue() != "POST" ||
        httpSettings.getProperty("Return Type Mediatype").getValue() != "Default" ||
        element.typeReference.getTypeId() != null ||
        element.getChildren().length !== 0
    ) {
        await dialogService.info(
            "When the \"Dapr Cron Binding\" stereotype is applied, Commands must adhere to the following constraints for the binding to be able to work:\n" +
            "\n" +
            "- The \"Http Settings\" stereotype's \"Verb\" must be set to \"POST\".\n" +
            "- The \"Http Settings\" stereotype's \"Return Type Mediatype\" must be set to \"Default\".\n" +
            "- It cannot have any fields.\n" +
            "- It cannot have a return type.");

        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Return Type Mediatype").setValue("Default");
        element.typeReference.setType(null);
        element.getChildren().forEach(child => child.delete());
    }
}

//await execute(element);