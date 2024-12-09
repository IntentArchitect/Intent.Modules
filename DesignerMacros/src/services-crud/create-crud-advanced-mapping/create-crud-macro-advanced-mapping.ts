/// <reference path="../_common/convertToAdvancedMapping.ts"/>
/// <reference path="../create-crud/create-crud.ts"/>

async function execute(element: IElementApi) {
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
    const service = existingService ?? createElement("Service", serviceName, package.id);

    const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
    const existingFolder = package.getChildren("Folder").find(x => x.getName() == pluralize(folderName));
    const folder = existingFolder ?? createElement("Folder", pluralize(folderName), package.id);
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);

    const resultDto = servicesCrud.createMappedResultDto(entity, folder);

    if (!privateSettersOnly || hasConstructor(entity)) {
        servicesCrud.createStandardCreateOperation(service, entity, folder);
    }

    if (primaryKeys.length > 0) {
        servicesCrud.createStandardFindByIdOperation(service, entity, resultDto);
    }

    servicesCrud.createStandardFindAllOperation(service, entity, resultDto);

    if (primaryKeys.length > 0) {
        if (!privateSettersOnly){
            servicesCrud.createStandardUpdateOperation(service, entity, folder);
        }
        servicesCrud.createStandardDeleteOperation(service, entity);

        const operations = DomainHelper.getCommandOperations(entity);     
        for (const operation of operations) {
            servicesCrud.createCallOperationCommand(service, operation, entity, folder);
        }

    }

    service.getChildren("Operation").forEach(operation => {
        convertToAdvancedMapping.convertOperation(operation, entity);
    })
}

function hasConstructor(entity: MacroApi.Context.IElementApi): boolean {
    return entity.getChildren("Class Constructor").length > 0;
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-crud/create-crud-advanced-mapping/create-crud-advanced-mapping.ts
 */
//await execute(element);