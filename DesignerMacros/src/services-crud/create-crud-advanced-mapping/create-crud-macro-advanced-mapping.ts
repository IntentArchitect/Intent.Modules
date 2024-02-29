/// <reference path="../_common/convertToAdvancedMapping.ts"/>
/// <reference path="../create-crud/create-crud.ts"/>

async function execute(element: IElementApi) {
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

    let resultDto = servicesCrud.createMappedResultDto(entity, folder);
    servicesCrud.createStandardCreateOperation(service, entity, folder);
    servicesCrud.createStandardFindByIdOperation(service, entity, resultDto);
    servicesCrud.createStandardFindAllOperation(service, entity, resultDto);
    servicesCrud.createStandardUpdateOperation(service, entity, folder);
    servicesCrud.createStandardDeleteOperation(service, entity);
 
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