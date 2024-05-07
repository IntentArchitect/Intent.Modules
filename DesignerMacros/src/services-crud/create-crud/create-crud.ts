// services-crud script (see ~/DesignerMacros/src/services-crud folder in Intent.Modules)
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../services-cqrs-crud/_common/onMapDto.ts" />

const privateSettersOnly = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";

namespace servicesCrud {
    export async function execute() {
        const package = element.getPackage();
        const entity = await DomainHelper.openSelectEntityDialog({
            includeOwnedRelationships: false
        });
        if (!entity) { return; }

        const serviceName = `${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
        const existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == pluralize(serviceName));
        const service = existingService || createElement("Service", serviceName, package.id);

        const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
        const existingFolder = package.getChildren("Folder").find(x => x.getName() == pluralize(folderName));
        const folder = existingFolder || createElement("Folder", pluralize(folderName), package.id);
        const primaryKeys = DomainHelper.getPrimaryKeys(entity);

        const resultDto = createMappedResultDto(entity, folder);
        createStandardCreateOperation(service, entity, folder);

        if (primaryKeys.length > 0) {
            createStandardFindByIdOperation(service, entity, resultDto);
        }

        createStandardFindAllOperation(service, entity, resultDto);

        if (primaryKeys.length > 0) {
            createStandardUpdateOperation(service, entity, folder);
            createStandardDeleteOperation(service, entity);

            const operations = DomainHelper.getCommandOperations(entity);     
            for (const operation of operations) {
                createCallOperationCommand(service, operation, entity, folder);
            }
    
        }
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


        onMapDto(dtoManager.getElement(), false, `${baseName}Create`);
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

        onMapDto(dtoManager.getElement(), false, `${baseName}Update`);

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

        onMapDto(dtoManager.getElement(), false, `${operation.getName()}`);

        dtoManager.collapse();
        operationManager.collapse();
    }


    function getBaseNameForElement(owningAggregate: IElementApi, entity: IElementApi, entityIsMany: boolean): string {
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