/// <reference path="servicesHelper.ts" />
/// <reference path="mappingStore.ts" />
/// <reference path="elementManager.ts" />
/// <reference path="../services-cqrs-crud/_common/onMapDto.ts" />

enum RepositoryCrudType {
    Create,
    Read,
    Update,
    Delete
}

interface IRepositoryOperationMetadata {
    crudType?: RepositoryCrudType,
    httpVerb?: string,
    httpRoute?: string,
    httpRouteParams: string[]
}

const mapToDomainOperationSettingId = "7c31c459-6229-4f10-bf13-507348cd8828";

class RepositoryServiceHelper {
    private static _createService(repository: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let serviceName = repository.getName();
        serviceName = RepositoryServiceHelper.sanitizeServiceName(serviceName);

        const existing = folder.getPackage().getChildren("Service").find(x => x.getName() == serviceName);
        if (existing) {
            return existing;
        }

        let serviceElement = createElement("Service", serviceName, folder.getPackage().id);
        return serviceElement;
    }

    public static sanitizeServiceName(name: string): string {
        name = removeSuffix(name, "Repository", "DAL");
        name += "Service";
        name = toPascalCase(name);
        return name;
    }

    public static createAppServiceOperationAction(
        operation: MacroApi.Context.IElementApi,
        folder: MacroApi.Context.IElementApi,
        service?: MacroApi.Context.IElementApi,
        syncElement: boolean = false
    ): MacroApi.Context.IElementApi {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);

        if (!service) {
            service = RepositoryServiceHelper._createService(operation.getParent(), folder);
        }

        // look up if there is an existing operation with the same name
        const existing = service.getChildren().find(x => x.getName() == operationName);

        // and return the existing one if the sync is disable (which is is by default)
        if (existing && !syncElement) {
            return existing;
        }

        let operationElement = existing ?? createElement("Operation", operationName, service.id);

        let mappingStore: MappingStore = new MappingStore();
        RepositoryServiceHelper.createAction(operation.getChildren("Parameter"), operationElement, false, folder, mappingStore, existing != null);

        // only add the association if not an existing operation
        if (!existing) {

            let callOp = createAssociation("Perform Invocation", operationElement.id, operation.id);
            let mapping = callOp.createAdvancedMapping(operationElement.id, operation.id);
            mapping.addMappedEnd("Invocation Mapping", [operationElement.id], [operation.id]);

            for (let entry of mappingStore.getMappings().reverse()) {
                mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
            }
        }

        if (!DomainHelper.isComplexType(operation.typeReference?.getType())) {
            operationElement.typeReference.setType(operation.typeReference.getTypeId());
            operationElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            operationElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        } else {
            var resultDto = RepositoryServiceHelper.createRepositoryClassTypeDto(operation, operation.typeReference?.getType(), folder);
            operationElement.typeReference.setType(resultDto.id);
            operationElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            operationElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }

        return operationElement;
    }

    public static createCqrsAction(operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, syncElement: boolean = false): MacroApi.Context.IElementApi {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async", "Command", "Query");
        operationName = toPascalCase(operationName);

        let metadata = RepositoryServiceHelper.getRepositoryOperationMetadata(operation);

        let actionTypeName;
        switch (metadata.crudType) {
            default:
                actionTypeName = "Command";
                break;
            case RepositoryCrudType.Read:
                actionTypeName = "Query";
                break;
        }

        const actionName = `${operationName}${actionTypeName}`;

        // if sync is set to true, then don't return right away
        const existing = folder.getChildren().find(x => x.getName() == actionName);
        if (existing && !syncElement) {
            return existing;
        }
        const actionElement = existing ?? createElement(actionTypeName, actionName, folder.id);

        let mappingStore: MappingStore = new MappingStore();
        RepositoryServiceHelper.createAction(operation.getChildren("Parameter"), actionElement, true, folder, mappingStore, existing != null);

        // don't recreate the association if it the entity exists exists
        if (!existing) {
            let callOp = createAssociation("Perform Invocation", actionElement.id, operation.id);
            let mapping = callOp.createAdvancedMapping(actionElement.id, operation.id);
            mapping.addMappedEnd("Invocation Mapping", [actionElement.id], [operation.id]);

            for (let entry of mappingStore.getMappings().reverse()) {
                mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
            }
        }

        if (!DomainHelper.isComplexType(operation.typeReference?.getType())) {
            actionElement.typeReference.setType(operation.typeReference.getTypeId());
            actionElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            actionElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        } else {
            var resultDto = RepositoryServiceHelper.createRepositoryClassTypeDto(operation, operation.typeReference?.getType(), folder);
            actionElement.typeReference.setType(resultDto.id);
            actionElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            actionElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }

        return actionElement;
    }

    private static getRepositoryOperationMetadata(operation: MacroApi.Context.IElementApi): IRepositoryOperationMetadata {
        let mappedElement = operation.getMapping()?.getElement();
        mappedElement
        let crudType: RepositoryCrudType;

        let httpSettings = mappedElement?.getStereotype("Http Settings");
        let httpVerb: string = httpSettings?.getProperty("Verb")?.getValue() as string;
        let httpRoute: string = httpSettings?.getProperty("Route")?.getValue() as string;
        const routeParamRegex = /\{([a-zA-Z0-9_\-]+)\}/g;
        let httpRouteParams = httpRoute ? [...httpRoute.matchAll(routeParamRegex)].map(match => match[1]) : [];

        if (httpVerb) {
            switch (httpVerb.toUpperCase()) {
                case "POST":
                    crudType = RepositoryCrudType.Create;
                    break;
                case "PUT":
                    crudType = RepositoryCrudType.Update;
                    break;
                case "DELETE":
                    crudType = RepositoryCrudType.Delete;
                    break;
                case "GET":
                    crudType = RepositoryCrudType.Read;
                    break;
            }
        } else if (mappedElement && (mappedElement.specialization === "Command" ||
            mappedElement.specialization === "Query" ||
            mappedElement.specialization === "Operation")) {
            for (let association of mappedElement.getAssociations()) {
                switch (association.specialization) {
                    case "Create Entity Action":
                        crudType = RepositoryCrudType.Create;
                        break;
                    case "Update Entity Action":
                        crudType = RepositoryCrudType.Update;
                        break;
                    case "Delete Entity Action":
                        crudType = RepositoryCrudType.Delete;
                        break;
                    case "Query Entity Action":
                        crudType = RepositoryCrudType.Read;
                        break;
                }
            }
        } else if (!crudType) {
            let mappedElementNameLower = (mappedElement ? mappedElement.getName() : operation.getName()).toLocaleLowerCase();
            if (mappedElementNameLower.indexOf("create") > -1) {
                crudType = RepositoryCrudType.Create;
            } else if (mappedElementNameLower.indexOf("update") > -1) {
                crudType = RepositoryCrudType.Update;
            } else if (mappedElementNameLower.indexOf("delete") > -1) {
                crudType = RepositoryCrudType.Delete;
            } else if (mappedElementNameLower.indexOf("get") > -1 || mappedElementNameLower.indexOf("find") > -1) {
                crudType = RepositoryCrudType.Read;
            }
        }

        return {
            crudType: crudType,
            httpVerb: httpVerb,
            httpRoute: httpRoute,
            httpRouteParams: httpRouteParams
        };
    }

    private static createAction(
        parameters: MacroApi.Context.IElementApi[],
        actionElement: MacroApi.Context.IElementApi,
        flattenFieldsFromComplexTypes: boolean,
        folder: MacroApi.Context.IElementApi,
        mappingStore: MappingStore,
        isExistingElement: boolean = false
    ): void {
        const childSpecialization = actionElement.specialization == "Operation" ? "Parameter" : "DTO-Field";
        let elementManager: ElementManager = new ElementManager(actionElement, { childSpecialization: childSpecialization });

        for (let repositoryField of parameters) {
            let paramRefType = repositoryField.typeReference?.getType()?.specialization;
            switch (paramRefType) {
                case "Class":
                case "Data Contract":
                case "Value Object":
                    let repositoryRefType = repositoryField.typeReference.getType();
                    if (flattenFieldsFromComplexTypes && !repositoryField.typeReference?.isCollection) {
                        mappingStore.pushTargetPath(repositoryField.id);
                        RepositoryServiceHelper.createAction(repositoryRefType.getChildren("Attribute"), elementManager.getElement(), false, folder, mappingStore, isExistingElement);
                        mappingStore.popTargetPath();
                    } else {
                        let actionField = elementManager.addChild(repositoryField.getName(), null);

                        mappingStore.pushSourcePath(actionField.id);
                        mappingStore.pushTargetPath(repositoryField.id);

                        let actionDto = RepositoryServiceHelper.replicateDto(repositoryRefType, folder, mappingStore, isExistingElement);

                        mappingStore.popSourcePath();
                        mappingStore.popTargetPath();

                        actionField.typeReference.setType(actionDto.id);
                        actionField.typeReference.setIsCollection(repositoryField.typeReference.isCollection);
                        actionField.typeReference.setIsNullable(repositoryField.typeReference.isNullable);
                        if (repositoryField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                            actionField.addMetadata("endpoint-input-id", repositoryField.getMetadata("endpoint-input-id"));
                        }

                        if (repositoryField.typeReference?.isCollection) {
                            actionField.setValue(actionDto.getValue());
                            if (!isExistingElement) {
                                mappingStore.addMapping(actionField.id, repositoryField.id);
                            }
                        }
                    }
                    break;
                default:
                    // Non-Complex type

                    // if mapping directly to a class, skip over the primary keys 
                    if (repositoryField.hasStereotype("Primary Key")) {
                        continue;
                    }

                    let fieldName = repositoryField.getName();
                    if (elementManager.getElement().getChildren().some(x => x.getName() === fieldName) && !isExistingElement) {
                        let parentName = repositoryField.getParent().getName();
                        fieldName = parentName + fieldName;
                    }
                    let actionField = elementManager.addChild(fieldName, repositoryField.typeReference);
                    actionField.setValue(repositoryField.getValue());

                    if (!isExistingElement) {
                        mappingStore.addMapping(actionField.id, repositoryField.id);
                    }
                    if (repositoryField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                        actionField.addMetadata("endpoint-input-id", repositoryField.getMetadata("endpoint-input-id"));
                    }
                    break;
            }
        }
        elementManager.collapse();
    }

    private static getBaseNameForElement(owningAggregate: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, entityIsMany: boolean): string {
        // Keeping 'owningAggregate' in case we still need to use it as part of the name one day
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }

    public static createRepositoryClassTypeDto(operation: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        let expectedDtoName = `${operation.getName()}${baseName}Dto`;

        let existing = folder.getChildren().find(x => x.getName() == expectedDtoName);
        if (existing) {
            return existing;
        }

        let dto = createElement("DTO", expectedDtoName, folder.id);

        dto.setMetadata("baseName", baseName);
        dto.setMapping(entity.id);

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            foreignKeys.forEach(fk => {
                let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(fk.name), dto.id);
                field.typeReference.setType(fk.typeId)
                if (fk.mapPath) {
                    field.setMapping(fk.mapPath);
                }
            })
        }

        ServicesHelper.addDtoFieldsFromDomain(dto, primaryKeys);

        let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
        for (var attr of attributesWithMapPaths) {
            if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) { continue; }
            let field = createElement("DTO-Field", attr.name, dto.id);
            field.typeReference.setType(attr.typeReferenceModel);
            field.setMapping(attr.mapPath);
        }

        onMapDto(dto, folder);
        dto.collapse();
        return dto;
    }

    private static replicateDto(repositoryDto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, mappingStore: MappingStore, isExistingElement: boolean = false): MacroApi.Context.IElementApi {
        var expectedName = `${repositoryDto.getName()}Dto`;

        // check to see if there is a DTO with the same name, and only if there is and we are working with an existing element do we update
        // this is to preserve backwards compatibility
        let existingDto = folder.getChildren().find(x => x.getName() == expectedName);
        let newDto = (existingDto && isExistingElement) ? existingDto : createElement("DTO", expectedName, folder.id);

        let elementManager: ElementManager = new ElementManager(newDto, { childSpecialization: "DTO-Field" });

        repositoryDto.getChildren("Attribute").forEach(repositoryField => {
            let existingField = newDto.getChildren().find(c => c.getName() == repositoryField.getName());
            //let actionField =  (existingField && isExistingElement) ? existingField :  createElement("DTO-Field", repositoryField.getName(), newDto.id);
            let actionField = (existingField && isExistingElement) ? existingField : elementManager.addChild(repositoryField.getName(), null);
            let fieldRefType = repositoryField.typeReference?.getType()?.specialization;

            switch (fieldRefType) {
                case "Class":
                case "Data Contract":
                case "Value Object": ``

                    // Complex type
                    mappingStore.pushSourcePath(actionField.id);
                    mappingStore.pushTargetPath(repositoryField.id);

                    let nestedDto = RepositoryServiceHelper.replicateDto(repositoryField.typeReference.getType(), folder, mappingStore, isExistingElement);

                    mappingStore.popSourcePath();
                    mappingStore.popTargetPath();

                    actionField.typeReference.setType(nestedDto.id);

                    if (repositoryField.typeReference?.isCollection) {
                        actionField.setValue(nestedDto.getValue());
                        mappingStore.addMapping(actionField.id, repositoryField.id);
                    }
                    break;
                default:
                    // Non-Complex type

                    // if mapping directly to a class, skip over the primary keys 
                    if (repositoryField.hasStereotype("Primary Key")) {
                        break;
                    }

                    actionField.typeReference.setType(repositoryField.typeReference.getTypeId());
                    actionField.setValue(repositoryField.getValue());
                    mappingStore.addMapping(actionField.id, repositoryField.id);
                    break;
            }

            actionField.typeReference.setIsCollection(repositoryField.typeReference.isCollection);
            actionField.typeReference.setIsNullable(repositoryField.typeReference.isNullable);
        });
        return newDto;
    }
}