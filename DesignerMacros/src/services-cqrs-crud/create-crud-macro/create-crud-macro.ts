/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../_common/onMapCommand.ts" />
/// <reference path="../_common/onMapDto.ts" />
/// <reference path="../_common/onMapQuery.ts" />
const privateSettersOnly = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";
const mapToDomainOperationSettingId = "7c31c459-6229-4f10-bf13-507348cd8828";

namespace cqrsCrud {

    export async function execute(element: IElementApi) {
        let entity = await DomainHelper.openSelectEntityDialog();
        if (entity == null) {
            return;
        }

        const owningEntity = DomainHelper.getOwningAggregate(entity);
        const folderName = pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? owningEntity.getName() : entity.getName());
        const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);

        const resultDto = createCqrsResultTypeDto(entity, folder);

        if (owningEntity == null || !privateSettersOnly) {
            createCqrsCreateCommand(entity, folder);
        }

        createCqrsFindByIdQuery(entity, folder, resultDto);
        createCqrsFindAllQuery(entity, folder, resultDto);

        if (!privateSettersOnly) {
            createCqrsUpdateCommand(entity, folder);
        }

        const operations = entity.getChildren("Operation").filter(x => x.typeReference.getType() == null);
        for (const operation of operations) {
            createCqrsCallOperationCommand(entity, operation, folder);
        }

        if (owningEntity == null || !privateSettersOnly) {
            createCqrsDeleteCommand(entity, folder);
        }
    }

    export function createCqrsCreateCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let expectedCommandName = `Create${baseName}Command`;

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);

        if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
            let command = folder.getChildren().filter(x => x.getName() == expectedCommandName)[0];

            let returnType = primaryKeys[0].typeId;
            if (primaryKeys.length > 1) {
                returnType = null;
            }
            command.typeReference.setType(returnType);
            return command;
        }

        let commandManager = new ElementManager(createElement("Command", expectedCommandName, folder.id), {
            childSpecialization: "DTO-Field"
        });

        let entityCtor: MacroApi.Context.IElementApi = entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
                // In descending order:
                return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
            })[0];
        if (entityCtor != null) {
            commandManager.mapToElement(entityCtor, mapToDomainOperationSettingId);
            commandManager.getElement().setMapping([entity.id, entityCtor.id], mapToDomainOperationSettingId);
        } else if (!privateSettersOnly) {
            commandManager.mapToElement(entity);
        } else {
            console.warn(`Private Setters are enabled with no constructor present on entity '${entity.getName()}'. In order for '${expectedCommandName}' to map to that entity, either disable private setters or model a constructor element and try again.`);
        }
        commandManager.getElement().setMetadata("baseName", baseName);

        let surrogateKey = primaryKeys.length === 1;
        if (surrogateKey) {
            commandManager.setReturnType(primaryKeys[0].typeId);
        }

        if (entityCtor) {
            commandManager.addChildrenFrom(DomainHelper.getChildrenOfType(entityCtor, "Parameter")
                .filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service"));
        } else {
            if (!surrogateKey) {          
                let toAdd = primaryKeys.filter(x => DomainHelper.isUserSuppliedPrimaryKey(lookup(x.id)));
                ServicesHelper.addDtoFieldsFromDomain(commandManager.getElement(), toAdd);
            }
            commandManager.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));
            commandManager.addChildrenFrom(getMandatoryAssociationsWithMapPath(entity));
        }

        if (owningAggregate != null){
            addAggregatePkToCommandOrQuery(owningAggregate, commandManager.getElement());
        }
        onMapCommand(commandManager.getElement(), true, true);

        commandManager.collapse();
        return commandManager.getElement();
    }

    export function createCqrsFindByIdQuery(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, resultDto: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let expectedQueryName = `Get${baseName}ByIdQuery`;

        if (folder.getChildren().some(x => x.getName() == expectedQueryName)) {
            return folder.getChildren().find(x => x.getName() == expectedQueryName);
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

        if (owningAggregate != null){
            addAggregatePkToCommandOrQuery(owningAggregate, query);
        }

        onMapQuery(query);
        query.collapse();
        return query;
    }

    export function createCqrsFindAllQuery(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, resultDto: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, true);
        let expectedQueryName = `Get${baseName}Query`;

        if (folder.getChildren().some(x => x.getName() == expectedQueryName)) {
            return folder.getChildren().find(x => x.getName() == expectedQueryName);
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
            addAggregatePkToCommandOrQuery(owningAggregate, query);
        }

        query.collapse();
        return query;
    }

    export function createCqrsUpdateCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let expectedCommandName = `Update${baseName}Command`;

        if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
            return folder.getChildren().find(x => x.getName() == expectedCommandName);
        }

        let command = new ElementManager(createElement("Command", expectedCommandName, folder.id), {
            childSpecialization: "DTO-Field"
        });
        command.mapToElement(entity);
        command.getElement().setMetadata("baseName", baseName);

        command.addChildrenFrom(DomainHelper.getAttributesWithMapPath(entity));
        command.addChildrenFrom(getMandatoryAssociationsWithMapPath(entity));

        let primaryKeys = DomainHelper.getPrimaryKeys(entity);
        ServicesHelper.addDtoFieldsFromDomain(command.getElement(), primaryKeys);

        if (owningAggregate != null){
            addAggregatePkToCommandOrQuery(owningAggregate, command.getElement());
        }

        onMapCommand(command.getElement(), true);
        command.collapse();
        return command.getElement();
    }

    export function createCqrsCallOperationCommand(entity: MacroApi.Context.IElementApi, operation: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        const owningAggregate = DomainHelper.getOwningAggregate(entity);
        const baseName = owningAggregate?.getName() ?? "";

        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);

        const commandName = `${operationName}${entity.getName()}Command`;

        const existing = folder.getChildren().find(
            x => x.getName() == commandName ||
                x.getMapping()?.getElement()?.id === operation.id);
        if (existing) {
            return existing;
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

        if (owningAggregate != null){
            addAggregatePkToCommandOrQuery(owningAggregate, commandElement);
        }
    
        onMapCommand(commandElement, true);
        commandManager.collapse();
        return commandManager.getElement();
    }

    export function createCqrsDeleteCommand(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = getBaseNameForElement(owningAggregate, entity, false);
        let expectedCommandName = `Delete${baseName}Command`;

        if (folder.getChildren().some(x => x.getName() == expectedCommandName)) {
            return folder.getChildren().find(x => x.getName() == expectedCommandName);
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

        if (owningAggregate != null){
            addAggregatePkToCommandOrQuery(owningAggregate, command);
        }
        onMapCommand(command, true);
        command.collapse();
        return command;
    }


    export function createCqrsResultTypeDto(entity: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
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

    function addAggregatePkToCommandOrQuery(owningAggregate: MacroApi.Context.IElementApi, commandOrQuery: MacroApi.Context.IElementApi) : void{
        if (owningAggregate != null){
            var aggPks = DomainHelper.getPrimaryKeys(owningAggregate);
            aggPks.forEach(x => {
                if (x.name.toLowerCase() == "id" ) 
                {
                    x.name = `${owningAggregate.getName()}Id`
                } 
                x.id = null;
                x.mapPath = null;
                x.isCollection = false;
                x.isNullable = false;
                });
                ServicesHelper.addDtoFieldsFromDomain(commandOrQuery,aggPks);
        }
    }    

    function getMandatoryAssociationsWithMapPath(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        return traverseInheritanceHierarchy(entity, [], []);

        function traverseInheritanceHierarchy(
            entity: MacroApi.Context.IElementApi,
            results: IAttributeWithMapPath[],
            generalizationStack: string[]
        ): IAttributeWithMapPath[] {
            entity
                .getAssociations("Association")
                .filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable && x.typeReference.isNavigable &&
                    !x.getOtherEnd().typeReference.isCollection && !x.getOtherEnd().typeReference.isNullable)
                .forEach(association => {
                    return results.push({
                        id: association.id,
                        name: association.getName(),
                        typeId: null,
                        mapPath: generalizationStack.concat([association.id]),
                        isNullable: false,
                        isCollection: false
                    });
                });


            let generalizations = entity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return results;
            }

            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);

            return traverseInheritanceHierarchy(generalization.typeReference.getType(), results, generalizationStack);
        }
    }

    function getBaseNameForElement(owningAggregate: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, entityIsMany: boolean): string {
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return owningAggregate ? `${toPascalCase(owningAggregate.getName())}${entityName}` : entityName;
    }
}
/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-crud-macro/create-crud-macro.ts
 */
//await cqrsCrud.execute(element);