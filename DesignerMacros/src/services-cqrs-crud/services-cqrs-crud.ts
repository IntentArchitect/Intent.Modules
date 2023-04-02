// services-cqrs-crud script (see ~/DesignerMacros/scr/services-cqrs-crud folder in Intent.Modules)
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
    createCqrsUpdateCommand(entity, folder);
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

    if (primaryKeys[0].typeId) {
        command.typeReference.setType(primaryKeys[0].typeId);
    }

    let attributesWithMapPaths = DomainHelper.getBusinessAttributes(entity);
    for (var attr of attributesWithMapPaths) {
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) { 
            continue;
        }
        let field = createElement("DTO-Field", attr.name, command.id);
        field.typeReference.setType(attr.typeId)
        field.setMapping(attr.mapPath);
    }

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


function createCqrsUpdateCommand(entity : MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    let owningAggregate = DomainHelper.getOwningAggregate(entity);
    let baseName = getBaseNameForElement(owningAggregate, entity, false);
    let expectedCommandName = `Update${baseName}Command`;

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

    let attributesWithMapPaths = DomainHelper.getBusinessAttributes(entity);
    for (var attr of attributesWithMapPaths) {
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) { continue; }
        let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(attr.name), command.id);
        field.typeReference.setType(attr.typeId)
        field.setMapping(attr.mapPath);
    }

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
    dto.setMapping(entity.id);
    dto.setMetadata("baseName", baseName);

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

    let attributesWithMapPaths = DomainHelper.getBusinessAttributes(entity);
    for (var attr of attributesWithMapPaths) {
        if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) { continue; }
        let field = createElement("DTO-Field", attr.name, dto.id);
        field.typeReference.setType(attr.typeId)
        field.setMapping(attr.mapPath);
    }

    dto.collapse();
    return dto;
}


function getBaseNameForElement(owningAggregate, entity, entityIsMany) : string {
    let entityName = entityIsMany ? toPascalCase(pluralize(entity.name)) : toPascalCase(entity.name);
    return owningAggregate ? `${toPascalCase(owningAggregate.name)}${entityName}` : entityName;
}
