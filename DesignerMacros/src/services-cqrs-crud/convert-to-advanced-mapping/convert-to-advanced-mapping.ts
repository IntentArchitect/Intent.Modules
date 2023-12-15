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
            let mapping = action.createMapping(command.id, entity.id);
            mapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            mapContract("Data Mapping", command, [command.id], [target.id], mapping);
        } else if (command.getName().startsWith("Delete")) {
            let action = createAssociation("Delete Entity Action", command.id, entity.id);
            let mapping = action.createMapping(command.id, entity.id);

            // Query Entity Mapping
            addFilterMapping(mapping, command, entity);
            command.clearMapping();
        } else if (command.isMapped()) {
            let action = createAssociation("Update Entity Action", command.id, target.id);

            // Query Entity Mapping
            let queryMapping = action.createMapping(command.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7"); 
            addFilterMapping(queryMapping, command, entity);
            // Update Entity Mapping
            let updateMapping = action.createMapping(command.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a"); 
            if (target.id != entity.id) {
                updateMapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            }
            mapContract("Data Mapping", command, [command.id], [target.id], updateMapping);
        }
    }

    function addFilterMapping(mapping: MacroApi.Context.IElementToElementMappingApi, command: IElementApi, entity: IElementApi) : void{
        let pkFields = DomainHelper.getPrimaryKeys(entity);
        if (pkFields.length == 1) {
            let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("PrimaryKey")) || (x.getName() == "Id" || x.getName() == `${entity.getName()}Id`));
            let entityPk = entity.getChildren("Attribute").find(x => x.hasStereotype("Primary Key"));
            if (idField && (idField.isMapped() || entityPk)) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? [entityPk.id]);
                idField.clearMapping();
            }
        } else {
            pkFields.forEach(pk => {
                let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("PrimaryKey") && x.getMapping().getElement().getName() == pk.name) || (x.getName() == pk.name));
                if (idField) {
                    mapping.addMappedEnd("Filter Mapping", [idField.id], idField.getMapping()?.getPath().map(x => x.id) ?? [pk.id]);
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
        let mapping = action.createMapping(query.id, entity.id);
        mapContract("Filter Mapping", query, [query.id], [entity.id], mapping);
    }

    function mapContract(mappingType: string, dto: MacroApi.Context.IElementApi, sourcePath: string[], targetPathIds: string[], mapping: MacroApi.Context.IElementToElementMappingApi): void {
        console.log("mapContract: " + dto.getName())
        dto.getChildren("DTO-Field").filter(x => x.isMapped() && !fieldsToSkip(dto, x)).forEach(field => {
            if (field.typeReference.getType()?.specialization != "DTO" || field.typeReference.getIsCollection()) {
                mapping.addMappedEnd(mappingType, sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)))
            }
            if (field.typeReference.getType()?.specialization == "DTO") {
                mapContract(mappingType, field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping);
            }
            field.clearMapping();
        })
        dto.clearMapping();
    }

    function fieldsToSkip(dto: MacroApi.Context.IElementApi, field:MacroApi.Context.IElementApi) : boolean{
        return dto.specialization == "Command" && 
            field.getMapping().getElement().hasStereotype("Primary Key") && 
            (!field.getMapping().getElement().getStereotype("Primary Key").hasProperty("Data source") || field.getMapping().getElement().getStereotype("Primary Key").getProperty("Data source").value != "User supplied");
    }

}
/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/convert-to-advanced-mapping/convert-to-advanced-mapping.ts
 */
//convertToAdvancedMapping.execute();