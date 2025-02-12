/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/getDefaultRoutePrefix.ts" />
/// <reference path="../_common/getRouteParts.ts" />
/// <reference path="../../common/getMappedDomainElement.ts" />

function exposeAsHttpEndPoint(element: MacroApi.Context.IElementApi): void {
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module

    if (!element.getParent().hasStereotype(httpServiceSettingsId)) {
        element.getParent().addStereotype(httpServiceSettingsId);

        let serviceBaseName = removeSuffix(element.getParent().getName(), "Service");
        element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").setValue(getRoute(serviceBaseName));
    }

    let parentRoute = element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").getValue().toString();

    applyHttpSettingsToOperations(element, parentRoute);
}

function getRoute(serviceBaseName: string): string {
    return `${getDefaultRoutePrefix(true)}${kebabCaseAcronymCorrection(toKebabCase(serviceBaseName), serviceBaseName)}`;
}

function applyHttpSettingsToOperations(operation: MacroApi.Context.IElementApi, existingRoute: string = ``): void {
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"; // from WebApi module
    const parameterSettingsId = "d01df110-1208-4af8-a913-92a49d219552"; // from WebApi module
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";

    if (!operation.hasStereotype(httpSettingsId)) {
        operation.addStereotype(httpSettingsId);
    }

    // get the name of the service, based on auto CRUD creation convention
    let serviceDomain = ``;
    if(operation.getParent() != null) {
        var serviceName = operation.getParent().getName();
        serviceDomain = singularize(serviceName.replace(`Service`, ``))
    }

    // filter out some common phrases
    let toReplace = [
        `Query`, `Request`, `ById`, `Create`, `Update`, `Delete`, `Modify`, `Insert`, `Patch`, `Remove`, 
        `Add`, `Set`, `List`, `Command`, `Find`, `Get`
    ];

    let supplementAdditionalReplacement: string[] = [];
    existingRoute.split('/').forEach((replacement) => {
        if(replacement.length > 0) {
            supplementAdditionalReplacement.push(replacement[0].toUpperCase() + replacement.substring(1));
            supplementAdditionalReplacement.push(pluralize(replacement[0].toUpperCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(pluralize(replacement[0].toLowerCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(pluralize(replacement));
            supplementAdditionalReplacement.push(singularize(replacement[0].toUpperCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(singularize(replacement[0].toLowerCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(singularize(replacement));
        }
    });

    toReplace.push(...supplementAdditionalReplacement);
    
    let operationName = operation.getName();
    toReplace.sort((a, b) => b.length - a.length).forEach((search) => {
        operationName = operationName.replace(search, '');
    });

    // first check if its the standard default operations
    // if its not one of the "defaults" setup by the CRUD accelerator
    // then calculate the route
    const httpSettings = operation.getStereotype(httpSettingsId);
    if(operation.getName() === `Create${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(``)
    } else if (operation.getName() === `Update${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("PUT");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName() === `Delete${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("DELETE");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName() === `Find${serviceDomain}ById`) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    }else if (operation.getName() === `Find${pluralize(serviceDomain)}`) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `{id}` : "")}`)
    } else if (operation.getName().startsWith("Get") || operation.getName().startsWith("Find") || operation.getName().startsWith("Lookup")) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`)
    } else if (operation.typeReference.getType() != null) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`)
    } else {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}`)
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
