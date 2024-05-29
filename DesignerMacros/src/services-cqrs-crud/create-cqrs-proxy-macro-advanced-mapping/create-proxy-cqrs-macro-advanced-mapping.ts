/// <reference path="../../common/openSelectElementDialog.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../_common/onMapCommand.ts" />

async function execute() {
    const selectedProxy = await openSelectElementDialog({
        elementType: "Service Proxy",
        packageTypeName: "Service Proxies"
    });

    const folderName = pluralize(selectedProxy.getName());
    const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);

    for (let operation of selectedProxy.getChildren("Operation")) {
        createCqrsCallOperationAction(operation, folder);
    }

    const diagramElement = folder.getChildren("Diagram").find(x => x.getName() == folderName) ?? createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    diagram.layoutVisuals(folder, null, true);
}

function createCqrsCallOperationAction(operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
    const parent = operation.getParent();

    let operationName = operation.getName();
    operationName = removeSuffix(operationName, "Async");
    operationName = toPascalCase(operationName);

    let metadata = getProxyOperationMetadata(operation);

    let actionTypeName;
    switch (metadata.crudType) {
        default:
            actionTypeName = "Command";
            break;
        case CrudType.Read:
            actionTypeName = "Query";
            break;
    }

    const actionName = `${operationName}${parent.getName()}${actionTypeName}`;

    const existing = folder.getChildren().find(
        x => x.getName() == actionName || x.getMapping()?.getElement()?.id === operation.id);
    if (existing) {
        return existing;
    }

    const actionElement = createElement(actionTypeName, actionName, folder.id);

    const actionElementManager = new ElementManager(actionElement, { childSpecialization: "DTO-Field" });
    
    const flattenTopLevelComplexType: boolean = true;
    recreateAction(operation.getChildren("Parameter"), flattenTopLevelComplexType, actionElementManager, folder);

    // commandManager.addChildrenFrom(DomainHelper.getChildrenOfType(operation, "Parameter")
    //     .filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service"));

    // if (owningAggregate != null) {
    //     addAggregatePkToCommandOrQuery(owningAggregate, commandElement);
    // }

    //onMapCommand(commandElement, true);
    actionElementManager.collapse();
    return actionElementManager.getElement();
}

enum CrudType {
    Create,
    Read,
    Update,
    Delete
}

interface IProxyOperationMetadata {
    crudType?: CrudType,
    httpVerb?: string,
    httpRoute?: string,
    httpRouteParams: string[]
}

function getProxyOperationMetadata(operation: MacroApi.Context.IElementApi): IProxyOperationMetadata {
    let mappedElement = operation.getMapping()?.getElement();
    let crudType: CrudType;

    if (mappedElement && (mappedElement.specialization === "Command" || 
        mappedElement.specialization === "Query" || 
        mappedElement.specialization === "Operation")) {
            for (let association of mappedElement.getAssociations()) {
                switch (association.specialization) {
                    case "Create Entity Action":
                        crudType = CrudType.Create;
                        break;
                    case "Update Entity Action":
                        crudType = CrudType.Update;
                        break;
                    case "Delete Entity Action":
                        crudType = CrudType.Delete;
                        break;
                    case "Query Entity Action":
                        crudType = CrudType.Read;
                        break;
                }
            }
    }

    if (!crudType) {
        let mappedElementNameLower = (mappedElement ? mappedElement.getName() : operation.getName()).toLocaleLowerCase();
        if (mappedElementNameLower.indexOf("create") > -1) {
            crudType = CrudType.Create;
        } else if (mappedElementNameLower.indexOf("update") > -1) {
            crudType = CrudType.Update;
        } else if (mappedElementNameLower.indexOf("delete") > -1) {
            crudType = CrudType.Delete;
        } else if (mappedElementNameLower.indexOf("get") > -1) {
            crudType = CrudType.Read;
        }
    }

    let httpSettings = mappedElement?.getStereotype("Http Settings");
    let httpVerb: string = httpSettings?.getProperty("Verb")?.getValue() as string;           
    let httpRoute: string = httpSettings?.getProperty("Route")?.getValue() as string;
    const routeParamRegex = /\{([a-zA-Z0-9_\-]+)\}/g;
    let httpRouteParams = httpRoute ? [...httpRoute.matchAll(routeParamRegex)].map(match => match[1]) : [];
    // let httpRouteParams = [];
    // let match;

    // while ((match = httpRoute.match(routeParamRegex))!== null) {
    //     httpRouteParams.push(...match.slice(1)); // Push all captured groups starting from the second element
    // }


    return {
        crudType: crudType,
        httpVerb: httpVerb,
        httpRoute: httpRoute,
        httpRouteParams: httpRouteParams
    };
}

function recreateAction(
        sourceFields: MacroApi.Context.IElementApi[], 
        flattenFieldsFromComplexTypes: boolean, 
        commandManager: ElementManager, 
        folder: MacroApi.Context.IElementApi) {

    for (let field of sourceFields) {
        let paramRefType = field.typeReference?.getType()?.specialization;
        switch (paramRefType) {
            case "Command":
            case "Query":
            case "DTO":
                // Complex type
                let referenceType = field.typeReference.getType();
                if (flattenFieldsFromComplexTypes) {
                    recreateAction(referenceType.getChildren("DTO-Field"), false, commandManager, folder);
                } else {
                    let newDto = replicateDto(referenceType, folder);
                    commandManager.addChild(field.getName(), newDto.id);
                }
                break;
            default:
                // Non-Complex type
                let fieldName = field.getName();
                if (commandManager.getElement().getChildren().some(x => x.getName() === fieldName)) {
                    let parentName = field.getParent().getName();
                    fieldName = parentName + fieldName;
                }
                commandManager.addChild(fieldName, field.typeReference);
                break;
        }
    }
}

function replicateDto(dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) : MacroApi.Context.IElementApi {
    let newDto = createElement("DTO", dto.getName(), folder.id);
    dto.getChildren("DTO-Field").forEach(field => {
        let newField = createElement("DTO-Field", field.getName(), newDto.id);
        let fieldRefType = field.typeReference?.getType()?.specialization;
        switch (fieldRefType) {
            case "Command":
            case "Query":
            case "DTO":
                // Complex type
                let nestedDto = replicateDto(field.typeReference.getType(), folder);
                newField.typeReference.setType(nestedDto.id);
                break;
            default:
                // Non-Complex type
                newField.typeReference.setType(field.typeReference.getTypeId());
                break;
        }
    });
    return newDto;
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-proxy-cqrs-macro-advanced-mapping/create-proxy-cqrs-macro-advanced-mapping.ts
 */
//await execute();