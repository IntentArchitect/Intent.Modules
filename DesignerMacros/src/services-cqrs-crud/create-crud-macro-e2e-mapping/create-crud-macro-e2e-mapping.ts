/// <reference path="../create-crud-macro/create-crud-macro.ts"/>
/// <reference path="../convert-to-e2e-mapping/convert-to-e2e-mapping.ts"/>

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
        convertToE2EMapping.convertCommand(cqrsCrud.createCqrsCreateCommand(entity, folder));
    }

    convertToE2EMapping.convertQuery(cqrsCrud.createCqrsFindByIdQuery(entity, folder, resultDto));
    convertToE2EMapping.convertQuery(cqrsCrud.createCqrsFindAllQuery(entity, folder, resultDto));

    if (!privateSettersOnly) {
        convertToE2EMapping.convertCommand(cqrsCrud.createCqrsUpdateCommand(entity, folder));
    }

    const operations = entity.getChildren("Operation").filter(x => x.typeReference.getType() == null);
    for (const operation of operations) {
        convertToE2EMapping.convertCommand(cqrsCrud.createCqrsCallOperationCommand(entity, operation, folder));
    }

    if (owningEntity == null || !privateSettersOnly) {
        convertToE2EMapping.convertCommand(cqrsCrud.createCqrsDeleteCommand(entity, folder));
    }
}


/**
 * Used by Intent.Modules\Modules\Intent.Modules.Application.MediatR.CRUD
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro-e2e-mapping/create-crud-macro-e2e-mapping.ts
 */
//await execute(element);