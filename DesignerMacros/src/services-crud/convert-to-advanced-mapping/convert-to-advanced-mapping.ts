/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
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
            if (dto?.isMapped || (element.getName().startsWith("Delete") && element.getChildren("Parameter").find(x => x.getName() == "id"))) {
                convertOperation(element, dto.getMapping().getElement());
            } else {
                console.warn("Cannot execute conversion script on Operation " + element.getName())
            }
        } 
        else {
            console.error("Cannot qualify this script. Please contact Intent Architect support.")
        }
    }

    export function convertOperation(operation: IElementApi, entity: IElementApi) {
        let target = entity;
        let dtoParam = operation.getChildren("Parameter").find(x => x.typeReference.getType().specialization == "DTO");
        let dto = dtoParam?.typeReference.getType();
        // CREATE OPERATION:
        if (operation.getName().startsWith("Create") && dtoParam?.typeReference.getType().getMapping()?.getElement().id == entity.id) {
            let action = createAssociation("Create Entity Action", operation.id, target.id);
            let mapping = action.createMapping(operation.id, entity.id);
            mapping.addMappedEnd("Invocation Mapping", [operation.id], [target.id]);
            mapContract("Data Mapping", dto, [operation.id, dtoParam.id], [target.id], mapping);
        // DELETE OPERATION:
        } else if (operation.getName().startsWith("Delete") && operation.getChildren("Parameter").find(x => x.getName() == "id")) {
            let action = createAssociation("Delete Entity Action", operation.id, entity.id);
            let mapping = action.createMapping(operation.id, entity.id);
            
            let idField = operation.getChildren("Parameter").find(x => x.getName() == "id");
            let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
            if (idField && entityPk) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], [entityPk.id]);
            }
        // UPDATE OPERATION:
        } else if (operation.getName().startsWith("Update") && dtoParam?.typeReference.getType().getMapping()?.getElement().id == entity.id) {
            let action = createAssociation("Update Entity Action", operation.id, target.id);

            // Query Entity Mapping
            let queryMapping = action.createMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7"); 
            let idField = operation.getChildren("Parameter").find(x => x.getName() == "id");
            let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
            if (idField && entityPk) {
                queryMapping.addMappedEnd("Filter Mapping", [idField.id], [entityPk.id]);
            }
            // Update Entity Mapping
            let updateMapping = action.createMapping(operation.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a"); 
            mapContract("Data Mapping", dto, [operation.id, dtoParam.id], [target.id], updateMapping);
        // FIND BY ID OPERATION:
        } else if (operation.getName().startsWith("Find" + entity.getName()) && operation.getChildren("Parameter").some(x => x.getName() == "id")) {
            let action = createAssociation("Query Entity Action", operation.id, target.id);
            let queryMapping = action.createMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7"); 
            let idField = operation.getChildren("Parameter").find(x => x.getName() == "id");
            let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
            if (idField && entityPk) {
                queryMapping.addMappedEnd("Filter Mapping", [idField.id], [entityPk.id]);
            }
        // FIND ALL OPERATION:
        } else if (operation.getName().startsWith("Find" + pluralize(entity.getName()))) {
            let action = createAssociation("Query Entity Action", operation.id, target.id);
            action.typeReference.setIsCollection(true);
            let queryMapping = action.createMapping(operation.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7"); 
            let idField = operation.getChildren("Parameter").find(x => x.getName() == "id");
            let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
            if (idField && entityPk) {
                queryMapping.addMappedEnd("Filter Mapping", [idField.id], [entityPk.id]);
            }
        }
    }

    function mapContract(mappingType: string, dto: MacroApi.Context.IElementApi, sourcePath: string[], targetPathIds: string[], mapping: MacroApi.Context.IElementToElementMappingApi, isNested: boolean = false): void {
        console.log("mapContract: " + dto.getName())
        dto.getChildren("DTO-Field").filter(x => x.isMapped() && (isNested || !x.getMapping().getElement().hasStereotype("Primary Key"))).forEach(field => {
            if (field.typeReference.getType()?.specialization != "DTO" || field.typeReference.getIsCollection()) {
                mapping.addMappedEnd(mappingType, sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)))
            }
            if (field.typeReference.getType()?.specialization == "DTO") {
                mapContract(mappingType, field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping, true);
            }
            field.clearMapping();
        })
        dto.clearMapping();
    }

}

convertToAdvancedMapping.execute();