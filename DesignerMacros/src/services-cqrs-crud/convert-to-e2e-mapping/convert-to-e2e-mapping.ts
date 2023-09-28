/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

let command = element;
if (command.isMapped()) {
    let entity = command.getMapping().getElement();
    if (command.getName().startsWith("Create")) {
        let action = createAssociation("Create Entity Action", command.id, entity.id);
        let mapping = action.createMapping(command.id, entity.id);
        mapContract(command, [command.id], [entity.id], mapping);
    } else if (command.getName().startsWith("Update")) {
        let entity = command.getMapping().getElement();
        let action = createAssociation("Update Entity Action", command.id, entity.id);
        let mapping = action.createMapping(command.id, entity.id);
        let idField = command.getChildren("DTO-Field").find(x => !x.isMapped() && (x.getName() == "Id" || x.getName() == `${entity.getName()}Id`));
        let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
        if (idField && entityPk) {
            mapping.addMappedEnd([idField.id], [entityPk.id]);
        }
        mapContract(command, [command.id], [entity.id], mapping);
    } else if (command.getName().startsWith("Delete")) {
        let action = createAssociation("Delete Entity Action", command.id, entity.id);
        let mapping = action.createMapping(command.id, entity.id);
        mapContract(command, [command.id], [entity.id], mapping);
    }
}

function mapContract(dto: MacroApi.Context.IElementApi, sourcePath: string[], targetPathIds: string[], mapping: MacroApi.Context.IElementToElementMappingApi): void {
    console.log("mapContract: " + dto.getName())
    dto.getChildren("DTO-Field").filter(x => x.isMapped()).forEach(field => {
        if (field.typeReference.getType().specialization != "DTO" || field.typeReference.getIsCollection()) {
            console.log("addMappedEnd: " + dto.getName() + "." + field.getName() + " - " + field.isMapped() + " : " + field.getMapping());
            mapping.addMappedEnd(sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)))
        }
        if (field.typeReference.getType().specialization == "DTO") {
            mapContract(field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping);
        }
        field.clearMapping();
    })
    dto.clearMapping();
}