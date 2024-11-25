/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/domainHelper.ts" />
namespace convertToAdvancedMapping {
    export function execute(): void {
        if (element.isMapped() && element.specialization == "Command") {
            convertCommand(element);
        } else if (element.isMapped() && element.specialization == "Query") {
            convertQuery(element);
        }
    }

    export function convertCommand(command: IElementApi): void {
        if (!command) {
            console.warn(`Could not convert null Command.`);
            return;
        }
        if (!command.getMapping()) {
            console.warn(`Could not convert Command '${command.getName()}' without it mapping to an Entity.`);
            return;
        }
        let target = command.getMapping().getElement();
        let entity = target.getParent("Class") ?? target;
        if (command.getName().startsWith("Create")) {
            let action = createAssociation("Create Entity Action", command.id, target.id);
            // JPS & GB: Updated the createMapping call to use createAdvancedMapping. If you are debugging
            // and this is not working, chat to JPS or GB
            let mapping = action.createAdvancedMapping(command.id, entity.id);
            mapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            mapContract("Data Mapping", command, command, [command.id], [target.id], mapping);
        } else if (command.getName().startsWith("Delete")) {
            let action = createAssociation("Delete Entity Action", command.id, entity.id);
            let mapping = action.createAdvancedMapping(command.id, entity.id);

            // Query Entity Mapping
            addFilterMapping(mapping, command, entity);
            command.clearMapping();
        } else if (command.isMapped()) {
            let action = createAssociation("Update Entity Action", command.id, target.id);

            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(command.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, command, entity);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(command.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            if (target.id != entity.id) {
                updateMapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            }
            mapContract("Data Mapping", command, command, [command.id], [target.id], updateMapping);
        }
    }

    function addFilterMapping(mapping: MacroApi.Context.IElementToElementMappingApi, command: IElementApi, entity: IElementApi): void {
        let pkFields = DomainHelper.getPrimaryKeys(entity);
        if (pkFields.length == 1) {
            let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key")) || (x.getName() == "Id" || x.getName() == `${entity.getName()}Id`));
            let entityPk = pkFields[0];
            if (idField && (idField.isMapped() || entityPk)) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? entityPk.mapPath ?? [entityPk.id]);
                idField.clearMapping();
            }
        } else {
            pkFields.forEach(pk => {
                let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key") && x.getMapping().getElement().getName() == pk.name) || (x.getName() == pk.name));
                if (idField) {
                    mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? pk.mapPath ?? [pk.id]);
                    idField.clearMapping();
                }
            });
        }
    }

    export function convertQuery(query: IElementApi) {
        if (!query) {
            console.warn(`Could not convert null Query.`);
            return;
        }
        if (!query.getMapping()) {
            console.warn(`Could not convert Query '${query.getName()}' without it mapping to an Entity.`);
            return;
        }
        let entity = query.getMapping().getElement();
        let action = createAssociation("Query Entity Action", query.id, entity.id);
        if (query.typeReference.getIsCollection()) {
            action.typeReference.setIsCollection(true);
        }
        let mapping = action.createAdvancedMapping(query.id, entity.id);
        mapContract("Filter Mapping", query, query, [query.id], [entity.id], mapping);
    }

    function mapContract(mappingType: string, root: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, sourcePath: string[], targetPathIds: string[], mapping: MacroApi.Context.IElementToElementMappingApi): void {

        if (dto.isMapped() && dto.getMapping().getElement().specialization == "Class Constructor"){
            if (targetPathIds[targetPathIds.length - 1] != dto.getMapping().getElement().id ){
                targetPathIds.push(dto.getMapping().getElement().id);
                //console.warn("Invocation Mapping : " + root.id + "->" + dto.getMapping().getElement().id);
                mapping.addMappedEnd("Invocation Mapping", [root.id], targetPathIds);
            }
        }
        dto.getChildren("DTO-Field").filter(x => x.isMapped() && !fieldsToSkip(dto, x)).forEach(field => {
            if (field.typeReference.getType()?.specialization != "DTO" || field.typeReference.getIsCollection()) {
                //console.warn("sourcePath : " + sourcePath);
                //console.warn("targetPathIds : " + targetPathIds);    
                //console.warn("sourceAdd : " + field.id);
                //console.warn("targetAdd : " + field.getMapping().getPath().map(x => x.id));    
                mapping.addMappedEnd(mappingType, sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)));
                updateElementWithMappedElement(field);
            }
            if (field.typeReference.getType()?.specialization == "DTO") {
                mapContract(mappingType, root, field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping);
            }
            field.clearMapping();
        })
        dto.clearMapping();
    }

    function fieldsToSkip(dto: MacroApi.Context.IElementApi, field: MacroApi.Context.IElementApi): boolean {
        return dto.specialization == "Command" &&
            field.getMapping().getElement().hasStereotype("Primary Key") &&
            (!field.getMapping().getElement().getStereotype("Primary Key").hasProperty("Data source") || field.getMapping().getElement().getStereotype("Primary Key").getProperty("Data source").value != "User supplied");
    }

    function updateElementWithMappedElement(field: MacroApi.Context.IElementApi) {
        let lastMappedPathElement = field.getMapping().getPath().slice(-1)[0];
        if (!lastMappedPathElement) {
            return;
        }
        let mappedElement = lastMappedPathElement.getElement();
        if (!mappedElement) {
            return;
        }
        field.typeReference.setIsNullable(mappedElement.typeReference.isNullable);
    }
}
