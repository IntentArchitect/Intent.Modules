/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../_common/onMapCommand.ts" />
/// <reference path="../_common/onMapDto.ts" />
/// <reference path="../_common/onMapQuery.ts" />
const privateSettersOnly = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";
const mapToDomainOperationSettingId = "7c31c459-6229-4f10-bf13-507348cd8828";

async function execute(element: IElementApi) {
    let entity = await DomainHelper.openSelectEntityDialog();
    if (entity == null) {
        return;
    }

    const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
    const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);

    const resultDto = createCqrsResultTypeDto(entity, folder);
    createCqrsCreateCommand(entity, folder);
    createCqrsFindByIdQuery(entity, folder, resultDto);
    createCqrsFindAllQuery(entity, folder, resultDto);

    if (!privateSettersOnly) {
        createCqrsUpdateCommand(entity, folder);
    }

    const operations = entity.getChildren("Operation").filter(x => x.typeReference.getType() == null);
    for (const operation of operations) {
        createCqrsCallOperationCommand(entity, operation, folder);
    }

    createCqrsDeleteCommand(entity, folder);

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

        let entityCtor: MacroApi.Context.IElementApi = entity
            .getChildren("Class Constructor")
            .filter(x => x.getChildren("Parameter").length > 0)
            .sort((a, b) => {
                // In descending order:
                return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
            })[0];
        if (entityCtor != null) {
            command.mapToElement(entityCtor, mapToDomainOperationSettingId);
            command.getElement().setMapping([entity.id, entityCtor.id], mapToDomainOperationSettingId);
        } else if (!privateSettersOnly) {
            command.mapToElement(entity);
        }
        command.getElement().setMetadata("baseName", baseName);

        if (primaryKeys[0].typeId) {
            command.setReturnType(primaryKeys[0].typeId);
        }

        if (entityCtor) {
            command.addChildrenFrom(DomainHelper.getChildrenOfType(entityCtor, "Parameter"));
        } else {
            command.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));
        }

        onMapCommand(command.getElement(), true, true);
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

        command.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));

        onMapCommand(command.getElement(), true);
        command.collapse();
    }

    function createCqrsCallOperationCommand(entity: MacroApi.Context.IElementApi, operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
        const owningAggregate = DomainHelper.getOwningAggregate(entity);
        const baseName = owningAggregate?.getName() ?? "";

        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);

        const commandName = `${operationName}${entity.getName()}Command`;

        if (folder.getChildren().some(
            x => x.getName() == commandName ||
                x.getMapping()?.getElement()?.id === operation.id)) {
            return;
        }

        const commandElement = createElement("Command", commandName, folder.id);
        commandElement.setMetadata("baseName", baseName);

        const commandManager = new ElementManager(commandElement, { childSpecialization: "DTO-Field" });
        commandManager.mapToElement([entity.id, operation.id], mapToDomainOperationSettingId);

        const primaryKeys = DomainHelper.getPrimaryKeys(entity);
        for (const key of primaryKeys) {
            commandManager.addChild(key.name, lookup(key.id).typeReference);
        }

        commandManager.addChildrenFrom(DomainHelper.getChildrenOfType(operation, "Parameter")
            .filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service"));

        onMapCommand(commandElement, true);
        commandManager.collapse();
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

        onMapCommand(command, true);
        command.collapse();
    }


    function createCqrsResultTypeDto(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
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
}

function getBaseNameForElement(owningAggregate: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, entityIsMany: boolean): string {
    let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
    return owningAggregate ? `${toPascalCase(owningAggregate.getName())}${entityName}` : entityName;
}

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Application.MediatR.CRUD
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro/create-crud-macro.ts
 */
//await execute(element);