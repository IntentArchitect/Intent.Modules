/// <reference path="../../services-cqrs-crud/create-crud-macro-advanced-mapping/create-crud-macro-advanced-mapping.ts"/>
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

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
            
            convertToAdvancedMapping.convertQuery(query);
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
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);
    if (command.getName().startsWith("Create") && !isOperationCommand) {
        let action = createAssociation("Create Entity Action", command.id, entity.id);
        let mapping = action.createAdvancedMapping(command.id, entity.id);
        mapping.addMappedEnd("Invocation Mapping", [command.id], [entity.id]);
        dataMapDtoToEntity(command, entity, [command.id], [entity.id], mapping, primaryKeys.map(x => x.id), []);
    } else if (command.getName().startsWith("Delete") && !isOperationCommand) {
        
    } else {
        let action = createAssociation("Update Entity Action", command.id, entity.id);
        let dataMapping = action.createAdvancedMapping(command.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
        dataMapDtoToEntity(command, entity, [command.id], [entity.id], dataMapping, primaryKeys.map(x => x.id), []);
    }
}

function dataMapDtoToEntity(dto: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, sourcePath: string[], targetPath: string[], mapping: MacroApi.Context.IElementToElementMappingApi, targetIdsToAvoid: string[], mappingChain: string[] = []) {
    // Check if this exact type is already being mapped in our current chain
    // This detects circular references like: A -> B -> A
    if (mappingChain.includes(dto.id)) {
        // We found a circular reference, stop this branch
        return;
    }
    for (let field of dto.getChildren("DTO-Field")) {
        let attribute = entity.getChildren("Attribute").filter(x => x.getName() === field.getName())[0];
        if (attribute && targetIdsToAvoid.indexOf(attribute.id) < 0) {
            mapping.addMappedEnd("Data Mapping", sourcePath.concat([field.id]), targetPath.concat([attribute.id]));
            continue;
        }

        let association = entity.getAssociations("Association").filter(x => x.getName() === field.getName())[0];
        if (association) {
            if (association.typeReference.isCollection) {
                mapping.addMappedEnd("Data Mapping", sourcePath.concat([field.id]), targetPath.concat([association.id]));
            }
            const primaryKeys = DomainHelper.getPrimaryKeys(association.typeReference.getType());
            // Add the current type to the chain when mapping nested objects
            dataMapDtoToEntity(
                field.typeReference.getType(), 
                association.typeReference.getType(), 
                sourcePath.concat([field.id]), 
                targetPath.concat([association.id]), 
                mapping, 
                primaryKeys.map(x => x.id), 
                [...mappingChain, dto.id] // Pass the current chain + this type
            );
            continue;
        }
    }
}

// Uncomment below
//await executeMapping();
