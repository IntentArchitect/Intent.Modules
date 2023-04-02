// services-cqs-crud script
async function execute() { 
    let entity = await preselectOrPromptEntity(null);
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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), command.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    if (primaryKeys[0].typeId) {
        command.typeReference.setType(primaryKeys[0].typeId);
    }

    let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { 
            continue;
        }
        let field = createElement("DTO-Field", entry.name, command.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), query.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    let primaryKeys = DomainHelper.getPrimaryKeys(entity);
    DomainHelper.addPrimaryKeysToElement(query, primaryKeys);

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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), query.id);
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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), command.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    let primaryKeys = DomainHelper.getPrimaryKeys(entity);
    DomainHelper.addPrimaryKeysToElement(command, primaryKeys);

    let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { continue; }
        let field = createElement("DTO-Field", DomainHelper.getFieldFormat(entry.name), command.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), command.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }


    let primaryKeys = DomainHelper.getPrimaryKeys(entity);
    DomainHelper.addPrimaryKeysToElement(command, primaryKeys);

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
            let field = createElement("DTO-Field", DomainHelper.getFieldFormat(fk.name), dto.id);
            field.typeReference.setType(fk.typeId)
            if (fk.mapPath) {
                field.setMapping(fk.mapPath);
            }
        })
    }

    DomainHelper.addPrimaryKeysToElement(dto, primaryKeys);

    let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { continue; }
        let field = createElement("DTO-Field", entry.name, dto.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
    }

    dto.collapse();
    return dto;
}


function getBaseNameForElement(owningAggregate, entity, entityIsMany) : string {
    let entityName = entityIsMany ? toPascalCase(pluralize(entity.name)) : toPascalCase(entity.name);
    return owningAggregate ? `${toPascalCase(owningAggregate.name)}${entityName}` : entityName;
}

async function preselectOrPromptEntity(preselectedDomainClassName?: string) {
    let classes = lookupTypesOf("Class").filter(x => DomainHelper.isAggregateRoot(x) || DomainHelper.ownerIsAggregateRoot(x) || x.hasStereotype("Repository"));
    if (classes.length == 0) {
        await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
        return;
    }

    let classId = await dialogService.lookupFromOptions(classes.map((x)=>({
        id: x.id, 
        name: getFriendlyDisplayNameForClassSelection(x)
        })));

    if (classId == null) {
        await dialogService.error(`No class found with id "${classId}".`);
        return null;
    }

    let foundEntity = lookup(classId);
    return foundEntity;
}


function getFriendlyDisplayNameForClassSelection(element) {
    let found = DomainHelper.getOwningAggregate(element);
    return !found ? element.getName() : `${element.getName()} (${found.getName()})`;
}
