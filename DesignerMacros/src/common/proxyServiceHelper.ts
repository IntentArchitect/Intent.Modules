/// <reference path="servicesHelper.ts" />
/// <reference path="mappingStore.ts" />
/// <reference path="elementManager.ts" />

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

class ProxyServiceHelper {
    private static _createService(proxy: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let serviceName = proxy.getName();
        serviceName = ProxyServiceHelper.sanitizeServiceName(serviceName);

        const existing = folder.getPackage().getChildren("Service").find(x => x.getName() == serviceName);
        if (existing) {
            return existing;
        }
        
        let serviceElement = createElement("Service", serviceName, folder.getPackage().id);
        //const httpServiceSettingsStereotypeId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";
        // let httpServiceSettings = serviceElement.getStereotype(httpServiceSettingsStereotypeId) ?? serviceElement.addStereotype(httpServiceSettingsStereotypeId);
        // httpServiceSettings.getProperty("Route").setValue(`api/${toKebabCase(serviceName)}`);
        return serviceElement;
    }

    public static sanitizeServiceName(name: string): string {
        name = removeSuffix(name, "Proxy", "Client", "Service");
        name = removeSuffix(name, "Proxy", "Client", "Service");
        name += "Service";
        name = toPascalCase(name);
        return name;
    }

    public static createAppServices(proxy: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let serviceElement = ProxyServiceHelper._createService(proxy, folder);

        for (let operation of proxy.getChildren("Operation")) {
            ProxyServiceHelper.createAppServiceOperationAction(operation, folder, serviceElement);
        }

        return serviceElement;
    }

    public static createAppServiceOperationAction(
        operation: MacroApi.Context.IElementApi, 
        folder: MacroApi.Context.IElementApi, 
        service?: MacroApi.Context.IElementApi,
        syncElement: boolean = false): MacroApi.Context.IElementApi {
            let operationName = operation.getName();
            operationName = removeSuffix(operationName, "Async");
            operationName = toPascalCase(operationName);

            let metadata = ProxyServiceHelper.getProxyOperationMetadata(operation);

            if (!service) {
                service = ProxyServiceHelper._createService(operation.getParent(), folder);
            }

            // look up the existing operation that invokes this proxy operation, falling back to one with the same name
            const target = ProxyServiceHelper.findExisting(service.getChildren(), operation, counter => `${operationName}${counter}`);
            const existing = target.existing;

            // and return the existing one if the sync is disable (which is is by default)
            if(existing && !syncElement){
                return existing;
            }

            let operationElement = existing ?? createElement("Operation", target.name, service.id);

            let verb = metadata.httpVerb ? metadata.httpVerb : "POST";
            let route = metadata.httpRoute ? metadata.httpRoute : `${toKebabCase(operationName)}`;
            
            // const httpSettingsStereotypeId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
            // let httpSettings = operationElement.getStereotype(httpSettingsStereotypeId) ?? operationElement.addStereotype(httpSettingsStereotypeId);
            // httpSettings.getProperty("Verb").setValue(verb);
            // httpSettings.getProperty("Route").setValue(route);

            let mappingStore: MappingStore = new MappingStore();
            ProxyServiceHelper.recreateAction(operation.getChildren("Parameter"), operationElement, false, folder, mappingStore, existing != null);

            // only add the association if not an existing operation
            if(!existing) {

                let callOp = createAssociation("Perform Invocation", operationElement.id, operation.id);
                let mapping = callOp.createAdvancedMapping(operationElement.id, operation.id);
                mapping.addMappedEnd("Invocation Mapping", [operationElement.id], [operation.id]);
                
                for (let entry of mappingStore.getMappings()) {
                    mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
                }
            }

            if (operation.typeReference?.getTypeId()) {
                operationElement.typeReference.setType(operation.typeReference.getTypeId());
                operationElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
                operationElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
            }

            return operationElement;
    }

    public static createCqrsAction(operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, syncElement: boolean = false): MacroApi.Context.IElementApi {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async", "Command", "Query");
        operationName = toPascalCase(operationName);

        let metadata = ProxyServiceHelper.getProxyOperationMetadata(operation);

        let actionTypeName;
        switch (metadata.crudType) {
            default:
                actionTypeName = "Command";
                break;
            case CrudType.Read:
                actionTypeName = "Query";
                break;
        }

        // look up the existing action that invokes this proxy operation, falling back to one with the same name
        const target = ProxyServiceHelper.findExisting(folder.getChildren(), operation, counter => `${operationName}${counter}${actionTypeName}`);
        const existing = target.existing;

        // if sync is set to true, then don't return right away
        if (existing && !syncElement) {
            return existing;
        }
        const actionElement = existing ?? createElement(actionTypeName, target.name, folder.id);

        // let verb = metadata.httpVerb ? metadata.httpVerb : "POST";
        // let route = metadata.httpRoute ? metadata.httpRoute : `api/${toKebabCase(folder.getName())}/${toKebabCase(actionName)}`;
        
        // const httpSettingsStereotypeId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
        // let httpSettings = actionElement.getStereotype(httpSettingsStereotypeId) ?? actionElement.addStereotype(httpSettingsStereotypeId);
        // httpSettings.getProperty("Verb").setValue(verb);
        // httpSettings.getProperty("Route").setValue(route);

        let mappingStore: MappingStore = new MappingStore();
        ProxyServiceHelper.recreateAction(operation.getChildren("Parameter"), actionElement, true, folder, mappingStore, existing != null);

        // don't recreate the association if it the entity exists exists
        if(!existing){
            let callOp = createAssociation("Perform Invocation", actionElement.id, operation.id);
            let mapping = callOp.createAdvancedMapping(actionElement.id, operation.id);
            mapping.addMappedEnd("Invocation Mapping", [actionElement.id], [operation.id]);
            
            for (let entry of mappingStore.getMappings()) {
                mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
            }
        }

        if (operation.typeReference?.getTypeId()) {
            actionElement.typeReference.setType(operation.typeReference.getTypeId());
            actionElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            actionElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }

        return actionElement;
    }

    // Proxy operations may share a name, so the element created for one is found by its Perform Invocation, not its name.
    // An element with the name we would use that invokes no proxy operation (made by hand, or before the association
    // existed) is still reused. When the name belongs to an element invoking a different proxy operation, the first free
    // numbered name is used instead (e.g. GetById2).
    private static findExisting(
        candidates: MacroApi.Context.IElementApi[],
        operation: MacroApi.Context.IElementApi,
        nameFor: (counter: string) => string): { existing?: MacroApi.Context.IElementApi, name: string } {
            const invoker = ProxyServiceHelper.findInvoker(candidates, operation);
            if (invoker) {
                return { existing: invoker, name: invoker.getName() };
            }

            for (let counter = 1; ; counter++) {
                const name = nameFor(counter == 1 ? "" : `${counter}`);
                const sameName = candidates.find(x => x.getName() == name);
                if (!sameName) {
                    return { existing: null, name: name };
                }
                if (!ProxyServiceHelper.invokesProxyOperation(sameName)) {
                    return { existing: sameName, name: name };
                }
            }
    }

    private static findInvoker(candidates: MacroApi.Context.IElementApi[], operation: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        return candidates.find(x => x.getAssociations("Perform Invocation")
            .some(a => a.isTargetEnd() && a.typeReference?.getTypeId() == operation.id));
    }

    private static invokesProxyOperation(element: MacroApi.Context.IElementApi): boolean {
        return element.getAssociations("Perform Invocation")
            .some(a => a.isTargetEnd() && a.typeReference?.getType()?.getParent()?.specialization == "Service Proxy");
    }

    private static getProxyOperationMetadata(operation: MacroApi.Context.IElementApi): IProxyOperationMetadata {
        let mappedElement = operation.getMapping()?.getElement();
        let crudType: CrudType;

        let httpSettings = mappedElement?.getStereotype("Http Settings");
        let httpVerb: string = httpSettings?.getProperty("Verb")?.getValue() as string;           
        let httpRoute: string = httpSettings?.getProperty("Route")?.getValue() as string;
        const routeParamRegex = /\{([a-zA-Z0-9_\-]+)\}/g;
        let httpRouteParams = httpRoute ? [...httpRoute.matchAll(routeParamRegex)].map(match => match[1]) : [];

        if (httpVerb) {
            switch (httpVerb.toUpperCase()) {
                case "POST":
                    crudType = CrudType.Create;
                    break;
                case "PUT":
                    crudType = CrudType.Update;
                    break;
                case "DELETE":
                    crudType = CrudType.Delete;
                    break;
                case "GET":
                    crudType = CrudType.Read;
                    break;
            }
        } else if (mappedElement && (mappedElement.specialization === "Command" || 
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
        } else if (!crudType) {
            let mappedElementNameLower = (mappedElement ? mappedElement.getName() : operation.getName()).toLocaleLowerCase();
            if (mappedElementNameLower.indexOf("create") > -1) {
                crudType = CrudType.Create;
            } else if (mappedElementNameLower.indexOf("update") > -1) {
                crudType = CrudType.Update;
            } else if (mappedElementNameLower.indexOf("delete") > -1) {
                crudType = CrudType.Delete;
            } else if (mappedElementNameLower.indexOf("get") > -1 || mappedElementNameLower.indexOf("find") > -1) {
                crudType = CrudType.Read;
            }
        }

        return {
            crudType: crudType,
            httpVerb: httpVerb,
            httpRoute: httpRoute,
            httpRouteParams: httpRouteParams
        };
    }

    private static recreateAction(
        proxyFields: MacroApi.Context.IElementApi[], 
        actionElement: MacroApi.Context.IElementApi,
        flattenFieldsFromComplexTypes: boolean, 
        folder: MacroApi.Context.IElementApi,
        mappingStore: MappingStore,
        isExistingElement: boolean = false): void {
            const childSpecialization = flattenFieldsFromComplexTypes 
                ? "DTO-Field" 
                : proxyFields.length > 0 
                    ? proxyFields[0].specialization 
                    : "DTO-Field";
            let elementManager: ElementManager = new ElementManager(actionElement, { childSpecialization: childSpecialization });

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
                            ProxyServiceHelper.recreateAction(proxyRefType.getChildren("DTO-Field"), elementManager.getElement(), false, folder, mappingStore, isExistingElement);
                            mappingStore.popTargetPath();
                        } else {
                            let actionField = elementManager.addChild(proxyField.getName(), null);

                            mappingStore.pushSourcePath(actionField.id);
                            mappingStore.pushTargetPath(proxyField.id);

                            let actionDto = ProxyServiceHelper.replicateDto(proxyRefType, folder, mappingStore, isExistingElement);

                            mappingStore.popSourcePath();
                            mappingStore.popTargetPath();

                            actionField.typeReference.setType(actionDto.id);
                            actionField.typeReference.setIsCollection(proxyField.typeReference.isCollection);
                            actionField.typeReference.setIsNullable(proxyField.typeReference.isNullable);
                            if (proxyField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                                actionField.addMetadata("endpoint-input-id", proxyField.getMetadata("endpoint-input-id"));
                            }
                        }
                        break;
                    default:
                        // Non-Complex type
                        let fieldName = proxyField.getName();
                        if (elementManager.getElement().getChildren().some(x => x.getName() === fieldName) && !isExistingElement) {
                            let parentName = proxyField.getParent().getName();
                            fieldName = parentName + fieldName;
                        }
                        let actionField = elementManager.addChild(fieldName, proxyField.typeReference);
                        actionField.setValue(proxyField.getValue());
                        if(!isExistingElement)
                        {
                            mappingStore.addMapping(actionField.id, proxyField.id);
                        }
                        if (proxyField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                            actionField.addMetadata("endpoint-input-id", proxyField.getMetadata("endpoint-input-id"));
                        }
                        break;
                }
            }
            elementManager.collapse();
    }

    private static replicateDto(proxyDto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, mappingStore: MappingStore, isExistingElement: boolean = false) : MacroApi.Context.IElementApi {

        // check to see if there is a DTO with the same name, and only if there is and we are working with an existing element do we update
        // this is to preseve backwards compatibility
        let existingDto = folder.getChildren().find(x => x.getName() == proxyDto.getName());
        let newDto = (existingDto && isExistingElement) ? existingDto : createElement("DTO", proxyDto.getName(), folder.id);

        proxyDto.getChildren("DTO-Field").forEach(proxyField => {
            let existingField = newDto.getChildren().find(c => c.getName() == proxyField.getName());
            let actionField =  (existingField && isExistingElement) ? existingField : createElement("DTO-Field", proxyField.getName(), newDto.id);
            let fieldRefType = proxyField.typeReference?.getType()?.specialization;
            switch (fieldRefType) {
                case "Command":
                case "Query":
                case "DTO":
                    // Complex type
                    mappingStore.pushSourcePath(actionField.id);
                    mappingStore.pushTargetPath(proxyField.id);

                    let nestedDto = ProxyServiceHelper.replicateDto(proxyField.typeReference.getType(), folder, mappingStore, isExistingElement);

                    mappingStore.popSourcePath();
                    mappingStore.popTargetPath();

                    actionField.typeReference.setType(nestedDto.id);
                    break;
                default:
                    // Non-Complex type
                    actionField.typeReference.setType(proxyField.typeReference.getTypeId());
                    actionField.setValue(proxyField.getValue());
                    mappingStore.addMapping(actionField.id, proxyField.id);
                    break;
            }

            actionField.typeReference.setIsCollection(proxyField.typeReference.isCollection);
            actionField.typeReference.setIsNullable(proxyField.typeReference.isNullable);
        });
        return newDto;
    }
}