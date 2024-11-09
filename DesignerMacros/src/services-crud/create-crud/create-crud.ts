// services-crud script (see ~/DesignerMacros/src/services-crud folder in Intent.Modules)
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/elementManager.ts" />
/// <reference path="../../services-cqrs-crud/_common/onMapDto.ts" />

const privateSettersOnly = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";

namespace servicesCrud {
    export async function execute() {
        const package = element.getPackage();
        const entity = await DomainHelper.openSelectEntityDialog({
            includeOwnedRelationships: false
        });
        if (!entity) { return; }

        if (privateSettersOnly && !hasConstructor(entity)) {
            await dialogService.warn(
`Partial Service Operation Creation.
Some service operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
        }

        const serviceName = `${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
        const existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == pluralize(serviceName));
        const service = existingService || createElement("Service", serviceName, package.id);

        const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
        const existingFolder = package.getChildren("Folder").find(x => x.getName() == pluralize(folderName));
        const folder = existingFolder || createElement("Folder", pluralize(folderName), package.id);
        const primaryKeys = DomainHelper.getPrimaryKeys(entity);

        const resultDto = createMappedResultDto(entity, folder);

        if (!privateSettersOnly || hasConstructor(entity)) {
            createStandardCreateOperation(service, entity, folder);
        }

        if (primaryKeys.length > 0) {
            createStandardFindByIdOperation(service, entity, resultDto);
        }

        createStandardFindAllOperation(service, entity, resultDto);

        if (primaryKeys.length > 0) {
            if (!privateSettersOnly){
                createStandardUpdateOperation(service, entity, folder);
            }
            createStandardDeleteOperation(service, entity);

            const operations = DomainHelper.getCommandOperations(entity);     
            for (const operation of operations) {
                createCallOperationCommand(service, operation, entity, folder);
            }
        }
    };

    function hasConstructor(entity: MacroApi.Context.IElementApi): boolean {
        return entity.getChildren("Class Constructor").length > 0;
    }

    export function createMappedResultDto(entity: IElementApi, folder: IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}Dto`;

        let existing = folder.getChildren().find(x => x.getName() == dtoName)
        if (existing) {
            return existing;
        }

        let result = CrudHelper.getOrCreateCrudDto(dtoName, entity, true, CrudConstants.dtoFromEntityMappingId, folder, false);
        onMapDto(result, folder);
        return result;

        /*
        let dtoManager = new ElementManager(createElement("DTO", dtoName, folder.id), {
            childSpecialization: "DTO-Field",
        });


        dtoManager.mapToElement(entity, ServicesConstants.dtoFromEntityMappingId);

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            dtoManager.addChildrenFrom(foreignKeys)
        }

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        dtoManager.addChildrenFrom(primaryKeys);
        dtoManager.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity))

        dtoManager.collapse();
        return dtoManager.getElement();*/
    }

    export function createStandardCreateOperation(service: IElementApi, entity: IElementApi, folder: IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}CreateDto`;

        if (service.getChildren().some(x => x.getName() == `Create${entity.getName()}`)) {
            let operation = service.getChildren().filter(x => x.getName() == `Create${entity.getName()}`)[0];
            let pks = DomainHelper.getPrimaryKeys(entity);
            operation.typeReference.setType(pks[0].typeId);
            return;
        }

        let dtoManager = new ElementManager(createElement("DTO", dtoName, folder.id), {
            childSpecialization: "DTO-Field",
        });
        let operationManager = new ElementManager(createElement("Operation", `Create${entity.getName()}`, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        })

        const entityCtor: MacroApi.Context.IElementApi = entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
                // In descending order:
                return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
            })[0];
        if (entityCtor != null) {
            dtoManager.mapToElement(entityCtor, ServicesConstants.dtoToDomainOperation);
            dtoManager.getElement().setMapping([entity.id, entityCtor.id], ServicesConstants.dtoToDomainOperation);
        } else if (!privateSettersOnly) {
            dtoManager.mapToElement(entity, ServicesConstants.dtoToEntityMappingId);
        } else {
            console.warn(`Private Setters are enabled with no constructor present on entity '${entity.getName()}'. In order for 'Create${entity.getName()}' to map to that entity, either disable private setters or model a constructor element and try again.`);
        }
        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        const surrogateKey = primaryKeys.length === 1;
        if (primaryKeys.length == 1) {
            operationManager.setReturnType(primaryKeys[0].typeId);
        }

        if (entityCtor) {
            dtoManager.addChildrenFrom(DomainHelper.getChildrenOfType(entityCtor, "Parameter")
                .filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service"));
        } else {
            if (!surrogateKey) {
                const toAdd = primaryKeys.filter(x => DomainHelper.isUserSuppliedPrimaryKey(lookup(x.id)));
                ServicesHelper.addDtoFieldsFromDomain(dtoManager.getElement(), toAdd);
            }
            dtoManager.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity))
            dtoManager.addChildrenFrom(DomainHelper.getMandatoryAssociationsWithMapPath(entity))
        }


        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            dtoManager.addChildrenFrom(foreignKeys)
            operationManager.addChildrenFrom(foreignKeys);
        }

        operationManager.addChild("dto", dtoManager.id);


        onMapDto(dtoManager.getElement(), folder, false, `${baseName}Create`, true);
        dtoManager.collapse();
        operationManager.collapse();
    }

    export function createStandardFindByIdOperation(service: IElementApi, entity: IElementApi, resultDto: IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let operationName = `Find${entity.getName()}ById`;

        if (service.getChildren().some(x => x.getName() == operationName)) {
            return;
        }
        let operationManager = new ElementManager(createElement("Operation", operationName, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        operationManager.setReturnType(resultDto.id)

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            operationManager.addChildrenFrom(foreignKeys);
        }

        operationManager.addChildrenFrom(DomainHelper.getPrimaryKeys(entity))

        operationManager.collapse();
    }


    export function createStandardFindAllOperation(service: IElementApi, entity: IElementApi, resultDto: IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let operationName = `Find${pluralize(entity.getName())}`;

        if (service.getChildren().some(x => x.getName() == operationName)) {
            return;
        }
        let operationManager = new ElementManager(createElement("Operation", operationName, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        operationManager.setReturnType(resultDto.id, true)

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            operationManager.addChildrenFrom(foreignKeys);
        }

        operationManager.collapse();
    }

    export function createStandardUpdateOperation(service: IElementApi, entity: IElementApi, folder: IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}UpdateDto`;

        if (service.getChildren().some(x => x.getName() == `Update${entity.getName()}`)) {
            let operation = service.getChildren().filter(x => x.getName() == `Update${entity.getName()}`)[0];
            let pks = DomainHelper.getPrimaryKeys(entity);
            operation.typeReference.setType(pks[0].typeId);
            return;
        }

        let dtoManager = new ElementManager(createElement("DTO", dtoName, folder.id), {
            childSpecialization: "DTO-Field",
            childType: "property"
        });
        let operationManager = new ElementManager(createElement("Operation", `Update${entity.getName()}`, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        dtoManager.mapToElement(entity, ServicesConstants.dtoToEntityMappingId);

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            dtoManager.addChildrenFrom(foreignKeys)
            operationManager.addChildrenFrom(foreignKeys);
        }

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        dtoManager.addChildrenFrom(primaryKeys);
        dtoManager.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity))
        dtoManager.addChildrenFrom(DomainHelper.getMandatoryAssociationsWithMapPath(entity))

        operationManager.addChildrenFrom(primaryKeys);
        operationManager.addChild("dto", dtoManager.id);

        onMapDto(dtoManager.getElement(), folder, false, `${baseName}Update`);

        dtoManager.collapse();
        operationManager.collapse();
    }


    export function createStandardDeleteOperation(service: IElementApi, entity: IElementApi) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);

        if (service.getChildren().some(x => x.getName() == `Delete${entity.getName()}`)) {
            return;
        }

        let operationManager = new ElementManager(createElement("Operation", `Delete${entity.getName()}`, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            operationManager.addChildrenFrom(foreignKeys);
        }

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);
        operationManager.addChildrenFrom(primaryKeys);

        operationManager.collapse();
    }

    export function createCallOperationCommand(service: MacroApi.Context.IElementApi, operation: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi){
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${operation.getName()}Dto`;

        if (service.getChildren().some(x => x.getName() == `${operation.getName()}`)) {
            return;
        }

        let dtoManager = new ElementManager(createElement("DTO", dtoName, folder.id), {
            childSpecialization: "DTO-Field",
            childType: "property"
        });
        let operationManager = new ElementManager(createElement("Operation", `${operation.getName()}`, service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        dtoManager.mapToElement(operation, ServicesConstants.dtoToDomainOperation);
        dtoManager.getElement().setMapping([entity.id, operation.id], ServicesConstants.dtoToDomainOperation);

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            dtoManager.addChildrenFrom(foreignKeys)
            operationManager.addChildrenFrom(foreignKeys);
        }

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        for (const key of primaryKeys) {
            dtoManager.addChild(key.name, lookup(key.id).typeReference);
        }
        dtoManager.addChildrenFrom(DomainHelper.getChildrenOfType(operation, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service"));

        operationManager.addChildrenFrom(primaryKeys);
        operationManager.addChild("dto", dtoManager.id);

        if (DomainHelper.isComplexType(operation.typeReference?.getType())) {
            var resultDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(operationManager.getElement(), operation) + "Result",  operation.typeReference?.getType(), false, CrudConstants.dtoFromEntityMappingId, folder, false);
            operationManager.setReturnType(resultDto.id, operation.typeReference.isCollection, operation.typeReference.isNullable);
        }
  
        onMapDto(dtoManager.getElement(), folder, false, `${operation.getName()}`);

        dtoManager.collapse();
        operationManager.collapse();
    }


    function getBaseNameForElement(owningAggregate: IElementApi, entity: IElementApi, entityIsMany: boolean): string {
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return owningAggregate ? `${toPascalCase(owningAggregate.getName())}${entityName}` : entityName;
    }
/*
    function getOrCreateCrudDto(
        command: MacroApi.Context.IElementApi,
        mappedElement: MacroApi.Context.IElementApi,
        autoAddPrimaryKey: boolean,
        mappingTypeSettingId: string,
        inbound: boolean = false
    ): MacroApi.Context.IElementApi {
        if (mappedElement.typeReference == null) throw new Error("TypeReference is undefined");

        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.typeReference.getType().getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        let dto = getOrCreateDto(dtoName, command.getParent());
        dto.setMetadata("originalVerb", originalVerb);
        dto.setMetadata("baseName", baseName);

        //dtoField.typeReference.setType(dto.id);
        const entityCtor: MacroApi.Context.IElementApi = mappedElement.typeReference.getType()
        .getChildren("Class Constructor")
        .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (inbound && entityCtor != null) {
            dto.setMapping([mappedElement.typeReference.getTypeId(), entityCtor.id], mapToDomainConstructorForDtosSettingId);
            addDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto);
        } else {
            dto.setMapping(mappedElement.typeReference.getTypeId(), mappingTypeSettingId);
            addDtoFields(autoAddPrimaryKey, mappedElement, dto);
        }

        return dto;
    }

    function addDtoFieldsForCtor(autoAddPrimaryKey: boolean, ctor: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
    
        let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");
    
        childrenToAdd.forEach(e => {
            if (e.mapPath != null) {
                if (dto.getChildren("Parameter").some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
    
            let field = createElement("DTO-Field", toPascalCase(e.name), dto.id);
            field.setMapping(e.mapPath);
            if (DomainHelper.isComplexTypeById(e.typeId)){
                let newDto = getOrCreateCommandCrudDto(dto, field, autoAddPrimaryKey, mapFromDomainMappingSettingId, true );
                field.typeReference.setType(newDto.id);
            }else{
                field.typeReference.setType(e.typeId);
            }
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
    
        dto.collapse();
    }
    
    function addDtoFields(autoAddPrimaryKey: boolean, mappedElement: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
        let dtoUpdated = false;
        let domainElement = mappedElement
            .typeReference
            .getType();
        let attributesWithMapPaths = getAttributesWithMapPath(domainElement);
        let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
        for (var keyName of Object.keys(attributesWithMapPaths)) {
            let entry = attributesWithMapPaths[keyName];
            if (isCreateMode && isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            field.typeReference.setType(entry.typeId);
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            field.setMapping(entry.mapPath);
            dtoUpdated = true;
        }
    
        if (autoAddPrimaryKey && !isCreateMode) {
            addPrimaryKeys(dto, domainElement, true);
        }    
    
        if (dtoUpdated) {
            dto.collapse();
        }
    }  */
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-crud/create-crud/create-crud.ts
 */
//await servicesCrud.execute();