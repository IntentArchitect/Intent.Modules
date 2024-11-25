/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

const serviceMvcSettingsId = "7bf5b988-bcc6-4e1f-8384-093fa04cbba5"; // From Intent.AspNetCore.Mvc module
const operationMvcSettingsId = "647586e0-09df-444a-9dc4-f13637b0c3ac"; // From Intent.AspNetCore.Mvc module

function getFindAllOperation(service: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
    for (const operation of service.getChildren("e030c97a-e066-40a7-8188-808c275df3cb")) {
        if (!operation.getName().startsWith("Find") || operation.typeReference?.getIsCollection() != true) {
            continue;
        }
    
        return operation;
    }

    return null;
}

function applyToOperation(operation: MacroApi.Context.IElementApi, findAllOperation: MacroApi.Context.IElementApi): void {
    if (!operation.hasStereotype(operationMvcSettingsId)) {
        operation.addStereotype(operationMvcSettingsId);
    }

    const mvcSettings = operation.getStereotype(operationMvcSettingsId);
    if (operation.getName().startsWith("Create")) {
        mvcSettings.getProperty("Verb").setValue("POST");
        mvcSettings.getProperty("Route").setValue(``);
    } else if (operation.getName().startsWith("Update")) {
        mvcSettings.getProperty("Verb").setValue("PUT");
        mvcSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName().startsWith("Delete")) {
        mvcSettings.getProperty("Verb").setValue("DELETE");
        mvcSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName().startsWith("Get") || operation.getName().startsWith("Find") || operation.getName().startsWith("Lookup")) {
        mvcSettings.getProperty("Verb").setValue("GET");
        mvcSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.typeReference.getType() != null) {
        mvcSettings.getProperty("Verb").setValue("GET");
        mvcSettings.getProperty("Route").setValue(`${toKebabCase(operation.getName())}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`)
    } else {
        mvcSettings.getProperty("Verb").setValue("POST");
        mvcSettings.getProperty("Route").setValue(`${toKebabCase(operation.getName())}`)
    }

    if (mvcSettings.getProperty("Verb").getValue() === "GET") {
        mvcSettings.getProperty("Return Type").setValue("View");
        return;
    }

    if (findAllOperation != null) {
        mvcSettings.getProperty("Return Type").setValue("RedirectToAction");
        mvcSettings.getProperty("RedirectTo Action").setValue(findAllOperation.id);
        return;
    }

    mvcSettings.getProperty("Return Type").setValue("Ok");
}
