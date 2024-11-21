/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/domainHelper.ts" />
namespace convertToAdvancedMapping {
    export function execute(): void {
        if (element.specialization == "Service") {
            let entity = element.getChildren("Operation").find(x => x.getName().startsWith("Find"))?.typeReference.getType()?.getMapping()?.getElement();
            element.getChildren("Operation").forEach(operation => {
                convertOperation(operation, entity);
            })
        }
        else if (element.specialization == "Operation") {
            let dtoParam = element.getChildren("Parameter").find(x => x.typeReference.getType().specialization == "DTO");
            let dto = dtoParam?.typeReference.getType() ?? element.typeReference?.getType();
            let entity = dto?.getMapping().getElement() ?? lookupTypesOf("Class").find(x => x.getName() == element.getName().replace("Delete", ""))
            if (entity) {
                convertOperation(element, entity);
            } else {
                console.warn("Cannot execute conversion script on Operation " + element.getName())
            }
        }
        else {
            console.error("Cannot qualify this script. Please contact Intent Architect support.")
        }
    }

    export function convertOperation(operation: IElementApi, entity: IElementApi) {
        let dtoParam = operation.getChildren("Parameter").find(x => x.typeReference.getType().specialization == "DTO");
        let dto = dtoParam?.typeReference.getType();
        let target = dto?.getMapping()?.getElement() ?? entity;
        let targetEntity = target.getParent("Class") ?? target;

        // CREATE OPERATION:
        if (operation.getName().startsWith("Create") /*&& dtoParam?.typeReference.getType().getMapping()?.getElement().id == entity.id*/) {
            let action = createAssociation("Create Entity Action", operation.id, target.id);
            let mapping = action.createAdvancedMapping(operation.id, targetEntity.id);
            mapping.addMappedEnd("Invocation Mapping", [operation.id], [target.id]);
            mapContract("Data Mapping", operation, dto, [operation.id, dtoParam.id], [target.id], mapping, true);
            // DELETE OPERATION:
        } else if (operation.getName().startsWith("Delete") && operation.getChildren("Parameter").find(x => x.getName().toLowerCase() == "id")) {
            let action = createAssociation("Delete Entity Action", operation.id, entity.id);
            let mapping = action.createAdvancedMapping(operation.id, entity.id);

            addFilterMapping(mapping, operation, entity);
            // UPDATE OPERATION:
        } else if (operation.getName().startsWith("Update") && dtoParam?.typeReference.getType().getMapping()?.getElement().id == entity.id) {
            let action = createAssociation("Update Entity Action", operation.id, target.id);

            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, operation, entity);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(operation.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            mapContract("Data Mapping", operation, dto, [operation.id, dtoParam.id], [target.id], updateMapping, true);
            // FIND BY ID OPERATION:
        } else if (operation.getName().startsWith("Find" + entity.getName()) && operation.getChildren("Parameter").some(x => x.getName().toLowerCase() == "id")) {
            let action = createAssociation("Query Entity Action", operation.id, target.id);
            let queryMapping = action.createAdvancedMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, operation, entity);
            // FIND ALL OPERATION:
        } else if (operation.getName().startsWith("Find" + pluralize(entity.getName()))) {
            let action = createAssociation("Query Entity Action", operation.id, target.id);
            action.typeReference.setIsCollection(true);
            let queryMapping = action.createAdvancedMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, operation, entity);
            //Operations (Command)
        } else if (dto.isMapped()){
            let action = createAssociation("Update Entity Action", operation.id, target.id);

            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, operation, entity);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(operation.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            updateMapping.addMappedEnd("Invocation Mapping", [operation.id], [target.id]);
            mapContract("Data Mapping",operation, dto, [operation.id, dtoParam.id], [target.id], updateMapping, true);

        } else {
            console.warn(`Could not convert operation: ${operation.getName()} (For entity ${entity.getName()}. Has parameters: (${operation.getChildren("Parameter").map(x => x.getName())}))`);
        }
    }

    function addFilterMapping(mapping: MacroApi.Context.IElementToElementMappingApi, operation: IElementApi, entity: IElementApi): void {
        let pkFields = DomainHelper.getPrimaryKeys(entity);
        if (pkFields.length == 1) {
            let idField = operation.getChildren("Parameter").find(x => x.getName().toLowerCase() == "id");
            let pk = pkFields[0];
            if (idField && pk) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], pk.mapPath ?? [pk.id]);
            }

        } else {
            pkFields.forEach(pk => {
                let idField = operation.getChildren("Parameter").find(x => (x.getName().toLowerCase() == pk.name.toLowerCase()));
                if (idField) {
                    mapping.addMappedEnd("Filter Mapping", [idField.id], pk.mapPath ?? [pk.id]);
                }
            });
        }
    }

    function mapContract(mappingType: string, root: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, sourcePath: string[], targetPathIds: string[], mapping: MacroApi.Context.IElementToElementMappingApi, isCommand: boolean = false): void {
        if (dto.isMapped() && dto.getMapping().getElement().specialization == "Class Constructor"){
            if (targetPathIds[targetPathIds.length - 1] != dto.getMapping().getElement().id ){
                targetPathIds.push(dto.getMapping().getElement().id);
                console.warn("Invocation Mapping : " + root.id + "->" + dto.getMapping().getElement().id);
                mapping.addMappedEnd("Invocation Mapping", [root.id], targetPathIds);
            }
        }
        dto.getChildren("DTO-Field").filter(x => x.isMapped() && !fieldsToSkip(isCommand, dto, x) ).forEach(field => {
            if (field.typeReference.getType()?.specialization != "DTO" || field.typeReference.getIsCollection()) {
                mapping.addMappedEnd(mappingType, sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)));
                updateElementWithMappedElement(field);
            }
            if (field.typeReference.getType()?.specialization == "DTO") {
                mapContract(mappingType, root, field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping, isCommand);
            }
            field.clearMapping();
        })
        dto.clearMapping();
    }

    function fieldsToSkip(isCommand:boolean, dto: MacroApi.Context.IElementApi, field: MacroApi.Context.IElementApi): boolean {
        return isCommand &&
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
