/// <reference path="../_common/convertToAdvancedMapping.ts"/>
/// <reference path="../create-crud/create-crud.ts"/>

async function execute(element: IElementApi) {
    const package = element.getPackage();

    let dialogResult = await CrudHelper.openCrudCreationDialog({
        includeOwnedRelationships: true,
        allowAbstract: false
    });

    if (!dialogResult) {
        return;
    }

    let entity: MacroApi.Context.IElementApi = dialogResult.selectedEntity;

    if (privateSettersOnly && !hasConstructor(entity) && dialogResult.canCreate) {
        await dialogService.warn(
`Partial Service Operation Creation.
Some service operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    }

    const serviceName = `${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
    const existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == pluralize(serviceName));
    const service = existingService ?? createElement("Service", serviceName, package.id);

    const targetFolder = getOrCreateEntityFolder(element, entity);
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);

    let resultDto: MacroApi.Context.IElementApi = null;
    if (dialogResult.canQueryById || dialogResult.canQueryAll) {
        resultDto = servicesCrud.createMappedResultDto(entity, targetFolder);
    }

    if ((!privateSettersOnly || hasConstructor(entity)) && dialogResult.canCreate) {
        servicesCrud.createStandardCreateOperation(service, entity, targetFolder);
    }

    if (primaryKeys.length > 0 && (!privateSettersOnly && dialogResult.canUpdate)) {
        servicesCrud.createStandardUpdateOperation(service, entity, targetFolder);
    }

    if (primaryKeys.length > 0 && dialogResult.canQueryById) {
        servicesCrud.createStandardFindByIdOperation(service, entity, resultDto);
    }

    if (dialogResult.canQueryAll) {
        servicesCrud.createStandardFindAllOperation(service, entity, resultDto);
    }

    if (primaryKeys.length > 0) {
        if (dialogResult.canDelete) {
            servicesCrud.createStandardDeleteOperation(service, entity);
        }

        if (dialogResult.canDomain) {
            const operations = DomainHelper.getCommandOperations(entity);     
            for (const operation of operations) {
                if (!dialogResult.selectedDomainOperationIds.some(x => x == operation.id)) {
                    continue;
                }
                servicesCrud.createCallOperationCommand(service, operation, entity, targetFolder);
            }
        }
    }

    service.getChildren("Operation").forEach(operation => {
        convertToAdvancedMapping.convertOperation(operation, entity);
    })
}

function getAggregateRootFolderName(entity: MacroApi.Context.IElementApi) {
    return pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
}

function getOrCreateEntityFolder(folderOrPackage: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
    if (folderOrPackage.specialization == "Folder") {
        return element;
    }

    const folderName = getAggregateRootFolderName(entity);
    const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);
    return folder;
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