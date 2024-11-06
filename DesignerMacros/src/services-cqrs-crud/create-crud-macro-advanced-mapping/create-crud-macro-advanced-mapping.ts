/// <reference path="../create-crud-macro/create-crud-macro.ts"/>
/// <reference path="../_common/convertToAdvancedMapping.ts"/>

async function execute(element: IElementApi, domainClass?: IElementApi) {

    let entity = !domainClass ? await DomainHelper.openSelectEntityDialog() : domainClass;
    if (entity == null) {
        return;
    }

    if (privateSettersOnly && !hasConstructor(entity)) {
        await dialogService.warn(
`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    }

    const owningEntity = DomainHelper.getOwningAggregate(entity);
    const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? owningEntity.getName() : entity.getName());
    const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);
    const hasPrimaryKey = primaryKeys.length > 0;

    const resultDto = cqrsCrud.createCqrsResultTypeDto(entity, folder);

    if (!privateSettersOnly || hasConstructor(entity)) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCreateCommand(entity, folder, primaryKeys));
    }

    if (hasPrimaryKey) {
        convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindByIdQuery(entity, folder, resultDto));
    }

    convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindAllQuery(entity, folder, resultDto));

    if (hasPrimaryKey && !privateSettersOnly) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsUpdateCommand(entity, folder));
    }

    const operations = DomainHelper.getCommandOperations(entity);     
    for (const operation of operations) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCallOperationCommand(entity, operation, folder));
    }

    if (hasPrimaryKey) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsDeleteCommand(entity, folder));
    }

    const diagramElement = folder.getChildren("Diagram").find(x => x.getName() == folderName) ?? createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    diagram.layoutVisuals(folder, null, true);
}


function hasConstructor(entity: MacroApi.Context.IElementApi): boolean {
    return entity.getChildren("Class Constructor").length > 0;
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro-advanced-mapping/create-crud-macro-advanced-mapping.ts
 */
//await execute(element);