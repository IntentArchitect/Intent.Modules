/// <reference path="../../common/domainHelper.ts" />

async function executeMapping() {
    const domainEntities = lookupTypesOf("Class");

    const entities = new Map<string, IElementApi>();
    for (const entity of domainEntities) {
        entities.set(entity.getName().toLowerCase(), entity);
    }

    let commands = lookupTypesOf("Command");

    let mappingsCreated = 0;
    const unmappedServices: string[] = [];

    for (const command of commands) {
        const commandName = command.getName().toLowerCase();
        let matched = false;

        const entityName = commandName
                .replace(/^(create|update|delete)/, "")
                .replace(/command$/, "")
                .toLowerCase();
        const entity = entities.get(entityName);
        
        if (entity) {
            mapCommandToEntity(command, entity);
            mappingsCreated++;
            matched = true;
        }

        if (!matched) {
            unmappedServices.push(command.getName());
        }
    }

    let queries = lookupTypesOf("Query");

    for (const query of queries) {
        const queryName = query.getName().toLowerCase();
        let matched = false;

        let entityName = queryName
            .replace(/^(get|find|list)/, "")
            .replace(/query$/, "")
            .replace(/byid$/i, "")
            .replace(/all$/i, "");
        entityName = singularize(entityName);
        const entity = entities.get(entityName);
        
        if (entity) {
            mapQueryToEntity(query, entity);
            mappingsCreated++;
            matched = true;
        }

        if (!matched) {
            unmappedServices.push(query.getName());
        }
    }

    if (mappingsCreated > 0) {
        await dialogService.info(`Successfully created ${mappingsCreated} advanced mappings.`);
    }

    if (unmappedServices.length > 0) {
        await dialogService.warn(
            `The following services could not be automatically mapped:\n${unmappedServices.join("\n")}`
        );
    }
}


function mapCommandToEntity(command: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi) {
    let isOperationCommand = command.hasMetadata("isOperationCommand") && command.getMetadata("isOperationCommand") == "true";
    if (command.getName().startsWith("Create") && !isOperationCommand) {
        let action = createAssociation("Create Entity Action", command.id, entity.id);
        let mapping = action.createAdvancedMapping(command.id, entity.id);
        mapping.addMappedEnd("Invocation Mapping", [command.id], [entity.id]);
        dataMapDtoToEntity(command, entity, [command.id], [entity.id], mapping, []);
    } else if (command.getName().startsWith("Delete") && !isOperationCommand) {
        let action = createAssociation("Delete Entity Action", command.id, entity.id);
        let queryMapping = action.createAdvancedMapping(command.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
        filterMapDtoToEntity(command, entity, queryMapping);
    } else {
        let action = createAssociation("Update Entity Action", command.id, entity.id);
        let queryMapping = action.createAdvancedMapping(command.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
        filterMapDtoToEntity(command, entity, queryMapping);
        let dataMapping = action.createAdvancedMapping(command.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
        dataMapDtoToEntity(command, entity, [command.id], [entity.id], dataMapping, []);
    }
}

function mapQueryToEntity(query: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi) {
    let action = createAssociation("Query Entity Action", query.id, entity.id);
    if (query.typeReference.getIsCollection()) {
        action.typeReference.setIsCollection(true);
    }
    let queryMapping = action.createAdvancedMapping(query.id, entity.id);
    filterMapDtoToEntity(query, entity, queryMapping);
}

function filterMapDtoToEntity(dto: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, mapping: MacroApi.Context.IElementToElementMappingApi) {
    let pkFields = DomainHelper.getPrimaryKeys(entity);
    if (pkFields.length == 1) {
        let idField = dto.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key")) || (x.getName() == "Id" || x.getName() == `${entity.getName()}Id`));
        let entityPk = pkFields[0];
        if (idField && (idField.isMapped() || entityPk)) {
            mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? entityPk.mapPath ?? [entityPk.id]);
        }
    } else {
        pkFields.forEach(pk => {
            let idField = dto.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key") && x.getMapping().getElement().getName() == pk.name) || (x.getName() == pk.name));
            if (idField) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? pk.mapPath ?? [pk.id]);
            }
        });
    }
}

function dataMapDtoToEntity(dto: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, sourcePath: string[], targetPath: string[], mapping: MacroApi.Context.IElementToElementMappingApi, mappingChain: string[] = []) {
    // Check if this exact type is already being mapped in our current chain
    // This detects circular references like: A -> B -> A
    if (mappingChain.includes(dto.id)) {
        // We found a circular reference, stop this branch
        return;
    }
    for (let field of dto.getChildren("DTO-Field")) {
        let attribute = entity.getChildren("Attribute").filter(x => x.getName() === field.getName())[0];
        var targetIdsToAvoid = DomainHelper.getPrimaryKeys(entity).map(x => x.id);
        if (attribute && targetIdsToAvoid.indexOf(attribute.id) < 0) {
            mapping.addMappedEnd("Data Mapping", sourcePath.concat([field.id]), targetPath.concat([attribute.id]));
            continue;
        }

        let association = entity.getAssociations("Association").filter(x => x.getName() === field.getName())[0];
        if (association) {
            if (association.typeReference.isCollection) {
                mapping.addMappedEnd("Data Mapping", sourcePath.concat([field.id]), targetPath.concat([association.id]));
            }
            // Add the current type to the chain when mapping nested objects
            dataMapDtoToEntity(
                field.typeReference.getType(), 
                association.typeReference.getType(), 
                sourcePath.concat([field.id]), 
                targetPath.concat([association.id]), 
                mapping, 
                [...mappingChain, dto.id] // Pass the current chain + this type
            );
            continue;
        }
    }
}

// Uncomment below
//await executeMapping();