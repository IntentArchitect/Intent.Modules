/// <reference path="../create-crud-macro/create-crud-macro.ts"/>
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../_common/convertToAdvancedMapping.ts"/>

async function execute(element: IElementApi, preselectedClass?: IElementApi) {

    let dialogResult: ICrudCreationResult = null;
    if (!preselectedClass) {
        dialogResult = await CrudHelper.openCrudCreationDialog({
            includeOwnedRelationships: true,
            allowAbstract: false
        });

        if (!dialogResult) {
            return;
        }
    } else {
        dialogResult = {
            selectedEntity: preselectedClass,
            canCreate: true,
            canUpdate: true,
            canQueryById: true,
            canQueryAll: true,
            canDelete: true,
            canDomain: true,
            selectedDomainOperationIds: []
        };
    }

    let entity: MacroApi.Context.IElementApi = dialogResult.selectedEntity;

    if ((privateSettersOnly && !hasConstructor(entity)) && dialogResult.canCreate) {
        await dialogService.warn(
`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    }

    const targetFolder = getOrCreateEntityFolder(element, entity);

    const primaryKeys = DomainHelper.getPrimaryKeys(entity);
    const hasPrimaryKey = primaryKeys.length > 0;
    
    let resultDto: MacroApi.Context.IElementApi = null;
    if (dialogResult.canQueryById || dialogResult.canQueryAll) {
        resultDto = cqrsCrud.createCqrsResultTypeDto(entity, targetFolder);
    }

    if ((!privateSettersOnly || hasConstructor(entity)) && dialogResult.canCreate) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCreateCommand(entity, targetFolder, primaryKeys));
    }

    if ((hasPrimaryKey && !privateSettersOnly) && dialogResult.canUpdate) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsUpdateCommand(entity, targetFolder));
    }

    if (hasPrimaryKey && dialogResult.canQueryById) {
        convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindByIdQuery(entity, targetFolder, resultDto));
    }

    if (dialogResult.canQueryAll) {
        convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindAllQuery(entity, targetFolder, resultDto));
    }

    if (hasPrimaryKey && dialogResult.canDelete) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsDeleteCommand(entity, targetFolder));
    }

    if (dialogResult.canDomain) {
        const operations = DomainHelper.getCommandOperations(entity);     
        for (const operation of operations) {
            if (!dialogResult.selectedDomainOperationIds.some(x => x == operation.id)) {
                continue;
            }
            convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCallOperationCommand(entity, operation, targetFolder));
        }
    }
    
    if (DomainHelper.isAggregateRoot(entity)) {
        const aggregateRootFolderName = getAggregateRootFolderName(entity);
        const diagramElement = targetFolder.getChildren("Diagram").find(x => x.getName() == aggregateRootFolderName) ?? createElement("Diagram", aggregateRootFolderName, targetFolder.id)
        diagramElement.loadDiagram();
        const diagram = getCurrentDiagram();
        diagram.layoutVisuals(targetFolder, null, true);
    }
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
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro-advanced-mapping/create-crud-macro-advanced-mapping.ts
 */
//await execute(element);