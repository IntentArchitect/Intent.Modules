// services-crud script (see ~/DesignerMacros/src/services-crud folder in Intent.Modules)
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
namespace servicesCrud {
    export async function execute() {
        let package = element.getPackage();
        let entity = await DomainHelper.openSelectEntityDialog({
            includeOwnedRelationships: false
        });
        if (!entity) { return; }

        let serviceName =`${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
        var existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == pluralize(serviceName));
        let service = existingService || createElement("Service", serviceName, package.id);
        
        let folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
        var existingFolder = package.getChildren("Folder").find(x => x.getName() == pluralize(folderName));
        var folder = existingFolder || createElement("Folder", pluralize(folderName), package.id);

        let resultDto = createMappedResultDto(entity, folder);
        createStandardCreateOperation(service, entity, folder);
        createStandardFindByIdOperation(service, entity, resultDto);
        createStandardFindAllOperation(service, entity, resultDto);
        createStandardUpdateOperation(service, entity, folder);
        createStandardDeleteOperation(service, entity);
    };

    export function createMappedResultDto(entity: IElementApi, folder: IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}Dto`;
        
        let existing = folder.getChildren().find(x => x.getName() == dtoName)
        if (existing) {
            return existing;
        }

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
        return dtoManager.getElement();
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

        dtoManager.mapToElement(entity, ServicesConstants.dtoToEntityMappingId);

        if (owningAggregate) {
            let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
            dtoManager.addChildrenFrom(foreignKeys)
            operationManager.addChildrenFrom(foreignKeys);
        }

        dtoManager.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity))

        operationManager.addChild("dto", dtoManager.id);

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);
        if (primaryKeys.length == 1) {
            operationManager.setReturnType(primaryKeys[0].typeId);
        }

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

        operationManager.addChildrenFrom(primaryKeys);
        operationManager.addChild("dto", dtoManager.id);

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

    function getBaseNameForElement(owningAggregate: IElementApi, entity: IElementApi, entityIsMany: boolean) : string {
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return owningAggregate ? `${toPascalCase(owningAggregate.getName())}${entityName}` : entityName;
    }

}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-crud/create-crud/create-crud.ts
 */
//await servicesCrud.execute();