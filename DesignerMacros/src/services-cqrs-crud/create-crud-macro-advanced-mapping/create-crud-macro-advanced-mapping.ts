/// <reference path="../create-crud-macro/create-crud-macro.ts"/>
/// <reference path="../convert-to-advanced-mapping/convert-to-advanced-mapping.ts"/>

async function execute(element: IElementApi) {
    let entity = await DomainHelper.openSelectEntityDialog();
    if (entity == null) {
        return;
    }

    const owningEntity = DomainHelper.getOwningAggregate(entity);
    const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? owningEntity.getName() : entity.getName());
    const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);

    const resultDto = cqrsCrud.createCqrsResultTypeDto(entity, folder);

    if (owningEntity == null || !privateSettersOnly) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCreateCommand(entity, folder));
    }

    convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindByIdQuery(entity, folder, resultDto));
    convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindAllQuery(entity, folder, resultDto));

    if (!privateSettersOnly) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsUpdateCommand(entity, folder));
    }

    const operations = entity.getChildren("Operation").filter(x => x.typeReference.getType() == null);
    for (const operation of operations) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCallOperationCommand(entity, operation, folder));
    }

    if (owningEntity == null || !privateSettersOnly) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsDeleteCommand(entity, folder));
    }

    const diagramElement = createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    diagram.layoutVisuals(folder, null, true);
}


/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro-advanced-mapping/create-crud-macro-advanced-mapping.ts
 */
//await execute(element);