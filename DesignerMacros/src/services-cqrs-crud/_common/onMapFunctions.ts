
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getSurrogateKeyType.ts" />

function getFieldFormat(str: string): string {
    return toPascalCase(str);
}

function getDomainAttributeNameFormat(str: string): string {
    let convention = getDomainAttributeNamingConvention();

    switch (convention) {
        case "pascal-case":
            return toPascalCase(str);
        case "camel-case":
            return toCamelCase(str);
        default:
            return str;
    }
}

function getOrCreateDto(elementName: string, parentElement: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
    const expectedDtoName = `${elementName}Dto`;
    let existingDto = parentElement.getChildren("DTO").filter(x => x.getName() === expectedDtoName)[0];
    if (existingDto) {
        return existingDto;
    }

    let dto = createElement("DTO", expectedDtoName, parentElement.id);
    return dto;
}

function ensureDtoFieldsForCtor(autoAddPrimaryKey: boolean, ctor: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
    
    let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");

    childrenToAdd.forEach(e => {
        if (e.mapPath != null) {
            if (dto.getChildren("Parameter").some(x => x.getMapping()?.getElement()?.id == e.id)) {
                return;
            }
        }
        else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
            return;
        }

        let field = createElement("DTO-Field", e.name, dto.id);
        field.typeReference.setType(e.typeId);
        field.typeReference.setIsCollection(e.isCollection);
        field.typeReference.setIsNullable(e.isNullable);

        if (this.mappedElement != null && e.mapPath) {
            field.setMapping(e.mapPath);
        }
    });

    dto.collapse();
}

function ensureDtoFields(autoAddPrimaryKey: boolean, mappedElement: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
    let dtoUpdated = false;
    let domainElement = mappedElement
        .typeReference
        .getType();
    let attributesWithMapPaths = getAttributesWithMapPath(domainElement);
    let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (isCreateMode && isOwnerForeignKey(entry.name, domainElement)) {
            continue;
        }
        if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
            continue;
        }
        let field = createElement("DTO-Field", toPascalCase( entry.name), dto.id);
        field.typeReference.setType(entry.typeId);
        field.typeReference.setIsNullable(entry.isNullable);
        field.typeReference.setIsCollection(entry.isCollection);
        field.setMapping(entry.mapPath);
        dtoUpdated = true;
    }

    if (autoAddPrimaryKey && !isCreateMode) {
        addPrimaryKeys(dto, domainElement, true);
    }    

    if (dtoUpdated) {
        dto.collapse();
    }
}

function isOwnerForeignKey(attributeName: string, domainElement: MacroApi.Context.IElementApi): boolean {
    for (let association of domainElement.getAssociations().filter(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable)) {
        if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
            return true;
        }
    }
    return false;
}

function addPrimaryKeys(dto: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, map: boolean): void {
    const primaryKeys = getPrimaryKeysWithMapPath(entity);

    for (const primaryKey of primaryKeys) {
        const name = getDomainAttributeNameFormat(primaryKey.name);
        if (dto.getChildren("DTO-Field").some(x => x.getName().toLowerCase() == name.toLowerCase())) {
            continue;
        }

        const dtoField = createElement("DTO-Field", getFieldFormat(name), dto.id);
        dtoField.typeReference.setType(primaryKey.typeId)

        if (map && primaryKey.mapPath != null) {
            console.log(`Doing mapping for ${dtoField.id}`);
            dtoField.setMapping(primaryKey.mapPath);
        }
    }
}

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean,
}

function getPrimaryKeysWithMapPath(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
    let keydict: { [index: string]: IAttributeWithMapPath } = Object.create(null);
    let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
    keys.forEach(key => keydict[key.id] = {
        id: key.id,
        name: key.getName(),
        typeId: key.typeReference.typeId,
        mapPath: [key.id],
        isNullable: false,
        isCollection: false
    });

    traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);

    return Object.values(keydict);

    function traverseInheritanceHierarchyForPrimaryKeys(
        keydict: { [index: string]: IAttributeWithMapPath },
        curEntity: MacroApi.Context.IElementApi,
        generalizationStack: string[]
    ): void {
        if (!curEntity) {
            return;
        }
        let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
        if (generalizations.length == 0) {
            return;
        }
        let generalization = generalizations[0];
        generalizationStack.push(generalization.id);
        let nextEntity = generalization.typeReference.getType();
        let baseKeys = nextEntity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        baseKeys.forEach(key => {
            keydict[key.id] = {
                id: key.id,
                name: key.getName(),
                typeId: key.typeReference.typeId,
                mapPath: generalizationStack.concat([key.id]),
                isNullable: key.typeReference.isNullable,
                isCollection: key.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
    }
}

function getAttributesWithMapPath(entity: MacroApi.Context.IElementApi): { [index: string]: IAttributeWithMapPath } {
    let attrDict: { [index: string]: IAttributeWithMapPath } = Object.create(null);
    let attributes = entity.getChildren("Attribute")
        .filter(x => !x.hasStereotype("Primary Key") &&
            !legacyPartitionKey(x) &&
            (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
    attributes.forEach(attr => attrDict[attr.id] = {
        id: attr.id,
        name: attr.getName(),
        typeId: attr.typeReference.typeId,
        mapPath: [attr.id],
        isNullable: false,
        isCollection: false
    });

    traverseInheritanceHierarchyForAttributes(attrDict, entity, []);

    return attrDict;

    function traverseInheritanceHierarchyForAttributes(attrDict: { [index: string]: IAttributeWithMapPath },
        curEntity: MacroApi.Context.IElementApi,
        generalizationStack: string[]
    ): void {
        if (!curEntity) {
            return;
        }
        let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
        if (generalizations.length == 0) {
            return;
        }
        let generalization = generalizations[0];
        generalizationStack.push(generalization.id);
        let nextEntity = generalization.typeReference.getType();
        let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !legacyPartitionKey(x));
        baseKeys.forEach(attr => {
            attrDict[attr.id] = {
                id: attr.id,
                name: attr.getName(),
                typeId: attr.typeReference.typeId,
                mapPath: generalizationStack.concat([attr.id]),
                isNullable: attr.typeReference.isNullable,
                isCollection: attr.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
    }
}

function getDomainAttributeNamingConvention(): "pascal-case" | "camel-case" {
    const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
    return <any>application.getSettings(domainSettingsId)
        ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
}

// Just in case someone still uses this convention. Used to filter out those attributes when mapping
// to domain entities that are within a Cosmos DB paradigm.
function legacyPartitionKey(attribute: MacroApi.Context.IElementApi): boolean {
    return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
}
