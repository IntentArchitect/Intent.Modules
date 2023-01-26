declare var globals;
(async () => {
//let element = lookup("29a81107-c71c-45b4-ba7b-982be63277a1")

const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";

var globals = initGlobals();

let fields = element.getChildren("DTO-Field")
    .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

fields.forEach(f => {
    let mappedElement = f.getMapping().getElement();

    let originalVerb = (element.getName().split(/(?=[A-Z])/))[0];

    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName") 
        ? `${element.getMetadata("baseName")}${domainName}`
        : domainName;
    let dtoName =  `${originalVerb}${baseName}`;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMetadata("originalVerb", originalVerb);
    dto.setMetadata("baseName", baseName);
    dto.setMapping(mappedElement.typeReference.getTypeId(), projectMappingSettingId);

    ensureDtoFields(mappedElement, dto);
    
    f.typeReference.setType(dto.id);
});

function getFieldFormat(str) : string {
    return toPascalCase(str);
}

function getDomainAttributeNameFormat(str) : string {
    let convention = getDomainAttributeNamingConvention();
    switch (convention) {
        case "pascal-case":
            return toPascalCase(str);
        case "camel-case":
            return toCamelCase(str);
    }
    return str;
}

function initGlobals() {
    return {
        PKSpecialization: {
            Implicit: "implicit",
            Explicit: "explicit",
            ExplicitComposite: "explicit_composite",
            Unknown: "unknown"
        },
        FKSpecialization: {
            Implicit: "implicit",
            Explicit: "explicit",
        }
    };
}

function getOrCreateDto(elementName, parentElement) {
    const expectedDtoName = `${elementName}Dto`;
    let existingDto = parentElement.getChildren("DTO").filter(x => x.name === expectedDtoName)[0];
    if (existingDto) {
        return existingDto;
    }

    let dto = createElement("DTO", expectedDtoName, parentElement.id);
    return dto;
}

function ensureDtoFields(mappedElement, dto) {
    let dtoUpdated = false;
    let domainElement = mappedElement
        .typeReference
        .getType();
    let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
    let attributesWithMapPaths = getAttributesWithMapPath(domainElement);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (isCreateMode && entry.name?.toLowerCase() === "id") {
            continue;
        }
        if (isCreateMode && isOwnerForeignKey(entry.name, domainElement)) {
            continue;
        }
        let field = createElement("DTO-Field", entry.name, dto.id);
        field.typeReference.setType(entry.typeId);
        field.typeReference.setIsNullable(entry.isNullable);
        field.typeReference.setIsCollection(entry.isCollection);
        field.setMapping(entry.mapPath);
        dtoUpdated = true;
    }

    if (!isCreateMode) {
        let entityPkDescr = getPrimaryKeyDescriptor(domainElement);
        addPrimaryKeys(dto, entityPkDescr);
    }

    if (dtoUpdated) {
        dto.collapse();
    }
}

function addPrimaryKeys(dto, entityPkDescr) {
    switch (entityPkDescr.specialization) {
        case globals.PKSpecialization.Implicit:
        case globals.PKSpecialization.Explicit:
            {
                let primaryKeyDtoField = createElement("DTO-Field", getFieldFormat(entityPkDescr.name), dto.id);
                primaryKeyDtoField.typeReference.setType(entityPkDescr.typeId);
                if (entityPkDescr.specialization == globals.PKSpecialization.Explicit) {
                    primaryKeyDtoField.setMapping(entityPkDescr.mapPath);
                }
            }
            break;
        case globals.PKSpecialization.ExplicitComposite:
            for (let key of entityPkDescr.compositeKeys) {
                let primaryKeyDtoField = createElement("DTO-Field", getFieldFormat(key.name), dto.id);
                primaryKeyDtoField.typeReference.setType(key.typeId)
                primaryKeyDtoField.setMapping(key.id);
            }
            break;
    }
}

function getSurrogateKeyType(): string {
    const commonTypes = {
        guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
        long: "33013006-E404-48C2-AC46-24EF5A5774FD",
        int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74"
    };
    const javaTypes = {
        long: "e9e575eb-f8de-4ce4-9838-2d09665a752d",
        int: "b3e5cb3b-8a26-4346-810b-9789afa25a82"
    };

    const typeNameToIdMap = new Map();
    typeNameToIdMap.set("guid", commonTypes.guid);
    typeNameToIdMap.set("int", lookup(javaTypes.int) != null ? javaTypes.int : commonTypes.int);
    typeNameToIdMap.set("long", lookup(javaTypes.long) != null ? javaTypes.long : commonTypes.long);

    let typeName = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Type")?.value ?? "int";
    if (typeNameToIdMap.has(typeName)) {
        return typeNameToIdMap.get(typeName);
    }

    return typeNameToIdMap.get("guid");
}

function isOwnerForeignKey(attributeName, domainElement) {
    for (let association of domainElement.getAssociations().filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable)) {
        if (attributeName.toLowerCase().indexOf(association.name.toLowerCase()) >= 0) {
            return true;
        }
    }
    return false;
}

// Returns a dictionary instead of element to help deal with explicit vs implicit keys
function getPrimaryKeyDescriptor(entity : MacroApi.Context.IElementApi) {
    if (!entity) {
        throw new Error("entity not specified");
    }
    let primaryKeys = getPrimaryKeysWithMapPath(entity);
    let keyLen = Object.keys(primaryKeys).length;

    switch (true) {
        case keyLen == 0:
            return {
                id: null,
                name: getDomainAttributeNameFormat("Id"),
                typeId: getSurrogateKeyType(),
                specialization: globals.PKSpecialization.Implicit,
                compositeKeys: null,
                mapPath: null
            };
        case keyLen == 1:
            let pkAttr = primaryKeys[Object.keys(primaryKeys)[0]];
            return {
                id: pkAttr.id,
                name: getDomainAttributeNameFormat(pkAttr.name),
                typeId: pkAttr.typeId,
                specialization: globals.PKSpecialization.Explicit,
                compositeKeys: null,
                mapPath: pkAttr.mapPath
            };
        case keyLen > 1:
            return {
                id: null,
                name: null,
                typeId: null,
                specialization: globals.PKSpecialization.ExplicitComposite,
                compositeKeys: Object.values(primaryKeys).map((v) => { 
                    return {
                        id: v.id,
                        name: getDomainAttributeNameFormat(v.name),
                        typeId: v.typeId,
                        mapPath: v.mapPath
                    }; 
                }),
                mapPath: null
            };
        default:
            return {
                id: null,
                name: null,
                typeId: null,
                specialization: globals.PKSpecialization.Unknown,
                compositeKeys: null,
                mapPath: null
            };
    }
}

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean
};

function getPrimaryKeysWithMapPath(entity : MacroApi.Context.IElementApi) {
    let keydict : { [characterName: string]: IAttributeWithMapPath} = Object.create(null);
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

    return keydict;

    function traverseInheritanceHierarchyForPrimaryKeys(
        keydict: { [characterName: string]: IAttributeWithMapPath }, 
        curEntity: MacroApi.Context.IElementApi, 
        generalizationStack) {
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

function getAttributesWithMapPath(entity : MacroApi.Context.IElementApi) {
    let attrDict : { [characterName: string]: IAttributeWithMapPath} = Object.create(null);
    let attributes = entity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !legacyPartitionKey(x));
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

    function traverseInheritanceHierarchyForAttributes(attrDict: { [characterName: string]: IAttributeWithMapPath }, 
        curEntity: MacroApi.Context.IElementApi, 
        generalizationStack) {
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

function getDomainAttributeNamingConvention() {
    const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
    return application.getSettings(domainSettingsId)
        ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
}

// Just in case someone still uses this convention. Used to filter out those attributes when mapping
// to domain entities that are within a Cosmos DB paradigm.
function legacyPartitionKey(attribute) {
    return attribute.hasStereotype("Partition Key") && attribute.name === "PartitionKey";
}
});