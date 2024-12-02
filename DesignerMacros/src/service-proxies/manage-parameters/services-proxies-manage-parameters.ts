/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
interface IHttpEndpointInputModel {
    id: string;
    name: string;
    typeId: string;
    isNullable: boolean;
    isCollection: boolean;
}

function GetParameters(
    targetService: IElementApi): IHttpEndpointInputModel[]
{
    let httpSettings = targetService.getStereotype("Http Settings")
    if (!httpSettings) {
        console.warn(`Could not process '${targetService.getName()}': Http Settings not found`)
        return [];
    }
    var isForCqrs = targetService.specialization == "Query" || targetService.specialization == "Command";
    var verbAllowsBody = ["PUT", "PATCH", "POST"].some(x => x == httpSettings.getProperty("Verb").getValue());
    var requiresBody = false;

    let result: IHttpEndpointInputModel[] = [];

    targetService.getChildren("Parameter").concat(targetService.getChildren("DTO-Field")).forEach(childElement => 
    {
        var parameterSettings = childElement.getStereotype("d01df110-1208-4af8-a913-92a49d219552"); // "Paremeter Settings"
        var routeContainsParameter = httpSettings.getProperty("Route").getValue().toString().toLowerCase().indexOf(`{${childElement.getName().toLowerCase()}}`) != -1;

        if (isForCqrs && !parameterSettings && !routeContainsParameter && verbAllowsBody)
        {
            requiresBody = true;
            return;
        }
        result.push({
            id: childElement.id,
            name: toCamelCase(childElement.getName()),
            typeId: childElement.typeReference.getTypeId(),
            isNullable: childElement.typeReference.getIsNullable(),
            isCollection: childElement.typeReference.getIsCollection(),
        })
    });

    if (isForCqrs && requiresBody)
    {
        result.push({
            id: targetService.id,
            name: targetService.specialization.toLowerCase(),
            typeId: targetService.id,
            isNullable: false,
            isCollection: false,
        })
    }

    return result;
}

function execute(proxyElement: IElementApi) {
    proxyElement.getChildren("Operation").forEach(operation => {
        let targetService = operation.getMapping().getElement();
        let params = GetParameters(targetService);
        params.forEach((param, index) => {
            let existing = operation.getChildren("Parameter").find(x => x.getMetadata("endpoint-input-id") == param.id)
            if (!existing) {
                existing = createElement("Parameter", param.name, operation.id);
                operation.collapse();
            }
            existing.setName(param.name, false);
            existing.setOrder(index);
            existing.typeReference.setType(param.typeId);
            existing.typeReference.setIsCollection(param.isCollection);
            existing.typeReference.setIsNullable(param.isNullable);
            existing.setMetadata("endpoint-input-id", param.id);
        })
        operation.getChildren("Parameter")
            .filter(x => params.every(p => p.id != x.getMetadata("endpoint-input-id")))
            .forEach(x => x.delete());


    });
}

/**
 * Used by Intent.Modules\Modules\Intent.Modelers.ServiceProxies
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/service-proxies/manage-parameters/service-proxies-manage-parameters.ts
 */
if (element.specialization == "Service Proxy") {
    execute(element);
} else {
    lookupTypesOf("Service Proxy").forEach(x => execute(x));
}