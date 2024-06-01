/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/mappingStore.ts" />

function createCqrsCallOperationAction(operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
    let operationName = operation.getName();
    operationName = removeSuffix(removeSuffix(removeSuffix(operationName, "Async"), "Command"), "Query");
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

    const actionName = `${operationName}${actionTypeName}`;

    const existing = folder.getChildren().find(
        x => x.getName() == actionName || x.getMapping()?.getElement()?.id === operation.id);
    if (existing) {
        return existing;
    }

    const actionElement = createElement(actionTypeName, actionName, folder.id);

    let verb = metadata.httpVerb ? metadata.httpVerb : "POST";
    let route = metadata.httpRoute ? metadata.httpRoute : `api/${toKebabCase(folder.getName())}/${toKebabCase(actionName)}`;
    
    const httpSettingsStereotypeId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let httpSettings = actionElement.getStereotype(httpSettingsStereotypeId) ?? actionElement.addStereotype(httpSettingsStereotypeId);
    httpSettings.getProperty("Verb").setValue(verb);
    httpSettings.getProperty("Route").setValue(route);

    const actionElementManager = new ElementManager(actionElement, { childSpecialization: "DTO-Field" });
    
    const flattenTopLevelComplexType: boolean = true;
    
    let mappingStore: MappingStore = new MappingStore();
    recreateAction(operation.getChildren("Parameter"), flattenTopLevelComplexType, actionElementManager, folder, mappingStore);

    let callOp = createAssociation("Call Service Operation", actionElement.id, operation.id);
    let mapping = callOp.createMapping(actionElement.id, operation.id);
    mapping.addMappedEnd("Invocation Mapping", [actionElement.id], [operation.id]);
    
    for (let entry of mappingStore.getMappings()) {
        mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
    }

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

    return {
        crudType: crudType,
        httpVerb: httpVerb,
        httpRoute: httpRoute,
        httpRouteParams: httpRouteParams
    };
}

function recreateAction(
        proxyFields: MacroApi.Context.IElementApi[], 
        flattenFieldsFromComplexTypes: boolean, 
        commandManager: ElementManager, 
        folder: MacroApi.Context.IElementApi,
        mappingStore: MappingStore): void {

    for (let proxyField of proxyFields) {
        let paramRefType = proxyField.typeReference?.getType()?.specialization;
        switch (paramRefType) {
            case "Command":
            case "Query":
            case "DTO":
                // Complex type
                let proxyRefType = proxyField.typeReference.getType();
                if (flattenFieldsFromComplexTypes) {
                    mappingStore.pushTargetPath(proxyField.id);
                    recreateAction(proxyRefType.getChildren("DTO-Field"), false, commandManager, folder, mappingStore);
                    mappingStore.popTargetPath();
                } else {
                    let actionField = commandManager.addChild(proxyField.getName(), null);

                    mappingStore.pushSourcePath(actionField.id);
                    mappingStore.pushTargetPath(proxyField.id);

                    let actionDto = replicateDto(proxyRefType, folder, mappingStore);

                    mappingStore.popSourcePath();
                    mappingStore.popTargetPath();

                    actionField.typeReference.setType(actionDto.id);
                    actionField.typeReference.setIsCollection(proxyField.typeReference.isCollection);
                    actionField.typeReference.setIsNullable(proxyField.typeReference.isNullable);
                }
                break;
            default:
                // Non-Complex type
                let fieldName = proxyField.getName();
                if (commandManager.getElement().getChildren().some(x => x.getName() === fieldName)) {
                    let parentName = proxyField.getParent().getName();
                    fieldName = parentName + fieldName;
                }
                let actionField = commandManager.addChild(fieldName, proxyField.typeReference);
                mappingStore.addMapping(actionField.id, proxyField.id);
                break;
        }
    }
}

function replicateDto(proxyDto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, mappingStore: MappingStore) : MacroApi.Context.IElementApi {
    let newDto = createElement("DTO", proxyDto.getName(), folder.id);
    proxyDto.getChildren("DTO-Field").forEach(proxyField => {
        let actionField = createElement("DTO-Field", proxyField.getName(), newDto.id);
        let fieldRefType = proxyField.typeReference?.getType()?.specialization;
        switch (fieldRefType) {
            case "Command":
            case "Query":
            case "DTO":
                // Complex type
                mappingStore.pushSourcePath(actionField.id);
                mappingStore.pushTargetPath(proxyField.id);

                let nestedDto = replicateDto(proxyField.typeReference.getType(), folder, mappingStore);

                mappingStore.popSourcePath();
                mappingStore.popTargetPath();

                actionField.typeReference.setType(nestedDto.id);
                break;
            default:
                // Non-Complex type
                actionField.typeReference.setType(proxyField.typeReference.getTypeId());
                mappingStore.addMapping(actionField.id, proxyField.id);
                break;
        }
    });
    return newDto;
}