/**
 * Used by Intent.Modules\Modules\Intent.Modules.Application.MediatR.CRUD
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro/create-crud-macro.ts
 */

/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../_common/command-on-map.ts" />
/// <reference path="../_common/dto-on-map.ts" />
/// <reference path="../_common/query-on-map.ts" />

async function execute() {
    let entity = await DomainHelper.openSelectEntityDialog();
    if (!entity) { return; }

    let folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
    var existing = element.getChildren().find(x => x.getName() == pluralize(folderName));
    var folder = existing || createElement("Folder", pluralize(folderName), element.id);

    let dto = createCqrsResultTypeDto(entity, folder);
    createCqrsCreateCommand(entity, folder);
    createCqrsFindByIdQuery(entity, folder, dto);
    createCqrsFindAllQuery(entity, folder, dto);
    const privateSetters = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";
    if (privateSetters) {
        let operations = entity.getChildren("Operation").filter(x => x.typeReference.getType() == null);
        operations.forEach(operation => {
            createCqrsCallOperationCommand(entity, operation, folder);
        })
    } else {
        createCqrsUpdateCommand(entity, folder);
    }
    createCqrsDeleteCommand(entity, folder);
};

function createCqrsCreateCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedCommandName = `Create${baseName}Command`;

    let primaryKeys = DomainHelper.getPrimaryKeys(entity);

    if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
        let command = folder.getChildren().filter(x => x.getName() == expectedCommandName)[0];
        command.typeReference.setType(primaryKeys[0].typeId);
        return;
    }

    let command = new ElementManager(createElement("Command", expectedCommandName, folder.id), {
        childSpecialization: "DTO-Field"
    });

    let entityCtor: MacroApi.Context.IElementApi = entity.getChildren("Class Constructor").find(x => x.getChildren("Parameter").length > 0);
    if (entityCtor) {
        command.mapToElement(entityCtor, "7c31c459-6229-4f10-bf13-507348cd8828"); // Map to Domain Operation
        command.getElement().setMapping([entity.id, entityCtor.id], "7c31c459-6229-4f10-bf13-507348cd8828") // Map to Domain Operation
    } else {
        command.mapToElement(entity);
    }
    command.getElement().setMetadata("baseName", baseName);

    if (owningAggregate) {
        command.addChildrenFrom(DomainHelper.getForeignKeys(entity, owningAggregate));
    }

    if (primaryKeys[0].typeId) {
        command.setReturnType(primaryKeys[0].typeId);
    }

    if (entityCtor) {
        command.addChildrenFrom(DomainHelper.getChildrenOfType(entityCtor, "Parameter"));
    } else {
        command.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));
    }

    onMapCommand(command.getElement());
    command.collapse();
}

function createCqrsFindByIdQuery(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, resultDto: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedQueryName = `Get${baseName}ByIdQuery`;

    if (folder.getChildren().some(x => x.getName() == expectedQueryName)) {
        return;
    }

    let query = createElement("Query", expectedQueryName, folder.id);
    query.typeReference.setType(resultDto.id)
    query.setMapping(entity.id);
    query.setMetadata("baseName", baseName);

    if (owningAggregate) {
        let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
        foreignKeys.forEach(fk => {
            let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(fk.name), query.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    let primaryKeys = DomainHelper.getPrimaryKeys(entity);
    ServicesHelper.addDtoFieldsFromDomain(query, primaryKeys);

    onMapQuery(query);
    query.collapse();
}

function createCqrsFindAllQuery(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, resultDto: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, true);
    let expectedQueryName = `Get${baseName}Query`;

    if (folder.getChildren().some(x => x.getName() == expectedQueryName)) {
        return;
    }

    let query = createElement("Query", expectedQueryName, folder.id);
    query.typeReference.setType(resultDto.id)
    query.typeReference.setIsCollection(true);
    query.setMapping(entity.id);
    query.setMetadata("baseName", baseName);

    if (owningAggregate) {
        let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
        foreignKeys.forEach(fk => {
            let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(fk.name), query.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    query.collapse();
}


function createCqrsUpdateCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedCommandName = `Update${baseName}Command`;

    if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
        return;
    }

    let command = new ElementManager(createElement("Command", expectedCommandName, folder.id), {
        childSpecialization: "DTO-Field"
    });
    command.mapToElement(entity);
    command.getElement().setMetadata("baseName", baseName);

    if (owningAggregate) {
        command.addChildrenFrom(DomainHelper.getForeignKeys(entity, owningAggregate));
    }

    command.addChildrenFrom(DomainHelper.getPrimaryKeys(entity));

    command.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));

    onMapCommand(command.getElement());
    command.collapse();
}

function createCqrsCallOperationCommand(entity: MacroApi.Context.IElementApi, operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = owningAggregate ? owningAggregate.getName() : "";
    let expectedCommandName = `${toPascalCase(operation.getName())}${baseName}Command`;

    if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
        return;
    }

    let command = new ElementManager(createElement("Command", expectedCommandName, folder.id), {
        childSpecialization: "DTO-Field"
    });
    command.mapToElement(operation);
    command.getElement().setMapping([entity.id, operation.id], "7c31c459-6229-4f10-bf13-507348cd8828") // Map to Domain Operation
    command.getElement().setMetadata("baseName", baseName);

    if (owningAggregate) {
        command.addChildrenFrom(DomainHelper.getForeignKeys(entity, owningAggregate).map(x => {
            x.mapPath = null;
            return x;
        }));
    }

    command.addChildrenFrom(DomainHelper.getPrimaryKeys(entity).map(x => {
        x.mapPath = null;
        return x;
    }));

    command.addChildrenFrom(DomainHelper.getChildrenOfType(operation, "Parameter"));

    onMapCommand(command.getElement());
    command.collapse();
}

function createCqrsDeleteCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedCommandName = `Delete${baseName}Command`;

    if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
        return;
    }

    let command = createElement("Command", expectedCommandName, folder.id);
    command.setMapping(entity.id);
    command.setMetadata("baseName", baseName);

    if (owningAggregate) {
        let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
        foreignKeys.forEach(fk => {
            let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(fk.name), command.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }


    let primaryKeys = DomainHelper.getPrimaryKeys(entity);
    ServicesHelper.addDtoFieldsFromDomain(command, primaryKeys);

    onMapCommand(command);
    command.collapse();
}


function createCqrsResultTypeDto(entity, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedDtoName = `${baseName}Dto`;

    let existing = folder.getChildren().find(x => x.getName() == expectedDtoName);
    if (existing) {
        return existing;
    }

    let dto = createElement("DTO", expectedDtoName, folder.id);
    dto.setMetadata("baseName", baseName);
    dto.setMapping(entity.id);

    let primaryKeys = DomainHelper.getPrimaryKeys(entity);

    if (owningAggregate) {
        let foreignKeys = DomainHelper.getForeignKeys(entity, owningAggregate);
        foreignKeys.forEach(fk => {
            let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(fk.name), dto.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    ServicesHelper.addDtoFieldsFromDomain(dto, primaryKeys);

    let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
    for (var attr of attributesWithMapPaths) {
        if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) { continue; }
        let field = createElement("DTO-Field", attr.name, dto.id);
        field.typeReference.setType(attr.typeId)
        field.setMapping(attr.mapPath);
    }

    onMapDto(dto);
    dto.collapse();
    return dto;
}


function getBaseNameForElement(owningAggregate, entity, entityIsMany): string {
    let entityName = entityIsMany ? toPascalCase(pluralize(entity.name)) : toPascalCase(entity.name);
    return owningAggregate ? `${toPascalCase(owningAggregate.name)}${entityName}` : entityName;
}
//await execute();