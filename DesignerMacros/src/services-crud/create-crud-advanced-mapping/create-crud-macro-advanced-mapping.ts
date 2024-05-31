/// <reference path="../_common/convertToAdvancedMapping.ts"/>
/// <reference path="../create-crud/create-crud.ts"/>

async function execute(element: IElementApi) {
    const package = element.getPackage();
    const entity = await DomainHelper.openSelectEntityDialog({
        includeOwnedRelationships: false
    });
    if (!entity) { return; }

    const serviceName = `${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
    const existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == pluralize(serviceName));
    const service = existingService ?? createElement("Service", serviceName, package.id);

    const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
    const existingFolder = package.getChildren("Folder").find(x => x.getName() == pluralize(folderName));
    const folder = existingFolder ?? createElement("Folder", pluralize(folderName), package.id);
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);

    const resultDto = servicesCrud.createMappedResultDto(entity, folder);
    servicesCrud.createStandardCreateOperation(service, entity, folder);

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

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-crud/create-crud-advanced-mapping/create-crud-advanced-mapping.ts
 */
//await execute(element);