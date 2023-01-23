declare var globals;
(async () => {
//let element = lookup("29a81107-c71c-45b4-ba7b-982be63277a1")

const projectMappingSettingId = "01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
const originalDtoMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";

initGlobals();

let fields = element.getChildren("DTO-Field")
    .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

fields.forEach(f => {
    let mappedElement = f.getMapping().getElement();

    let originalVerb = "";
    if (element.hasMetadata("originalVerb")) {
        originalVerb = element.getMetadata("originalVerb");
        // In the event that the prefix is no longer the same as the
        // originally called verb, then don't propagate this any further
        // as end users might get confused.
        if (element.getName().indexOf(originalVerb) < 0) {
            originalVerb = "";
        }
    }

    let targetMappingSettingId = (!originalVerb || originalVerb === "") 
        ? originalDtoMappingSettingId 
        : projectMappingSettingId;

    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName") 
        ? `${element.getMetadata("baseName")}${domainName}`
        : domainName;
    let dtoName =  `${originalVerb}${baseName}`;
    let dto = getOrCreateDTO(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), targetMappingSettingId);
    if (originalVerb !== "") {
        dto.setMetadata("originalVerb", originalVerb);
    }
    dto.setMetadata("baseName", baseName);
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

var globals: any;

function initGlobals() {
    globals = {
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

function getOrCreateDTO(elementName, parentElement) {
    let existingDto = parentElement.getChildren("DTO").filter(x => x.name === `${elementName}DTO`)[0];
    if (existingDto) {
        return existingDto;
    }

    let dto = createElement("DTO", `${elementName}DTO`, parentElement.id);
    return dto;
}

function ensureDtoFields(mappedElement, dto) {
    let dtoUpdated = false;
    let domainElement = mappedElement
        .typeReference
        .getType();
    let mappedElementAttributes = domainElement
        .getChildren("Attribute");
    let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
    let dtoFields = dto.getChildren("DTO-Field");
    for (let attribute of mappedElementAttributes.filter(x => ! dtoFields.some(y => x.name === y.name))) {
        if (isCreateMode && attribute.name?.toLowerCase() === "id") {
            continue;
        }
        if (isCreateMode && isOwnerForeignKey(attribute, domainElement)) {
            continue;
        }
        if (legacyPartitionKey(attribute)) {
            continue;
        }
        if (attribute.hasStereotype("Primary Key")) {
            continue;
        }
        let field = createElement("DTO-Field", attribute.name, dto.id);
        field.typeReference.setType(attribute.typeReference.typeId);
        field.typeReference.setIsNullable(attribute.typeReference.isNullable);
        field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        field.setMapping(attribute.id);
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

function getSurrogateKeyType() {
    const typeNameToIdMap = new Map();
    typeNameToIdMap.set("guid", "6b649125-18ea-48fd-a6ba-0bfff0d8f488");
    typeNameToIdMap.set("int", "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74");
    typeNameToIdMap.set("long", "33013006-E404-48C2-AC46-24EF5A5774FD");

    let typeName = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Type")?.value ?? "int";
    if (typeNameToIdMap.has(typeName)) {
        return typeNameToIdMap.get(typeName);
    }

    return typeNameToIdMap.get("guid");
}

function isOwnerForeignKey(attribute, domainElement) {
    for (let association of domainElement.getAssociations().filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable)) {
        if (attribute.name.toLowerCase().indexOf(association.name.toLowerCase()) >= 0) {
            return true;
        }
    }
    return false;
}

// Returns a dictionary instead of element to help deal with explicit vs implicit keys
function getPrimaryKeyDescriptor(entity) {
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

interface IPrimaryKey {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[]
};

function getPrimaryKeysWithMapPath(entity) {
    let keydict : { [characterName: string]: IPrimaryKey} = Object.create(null);
    let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
    keys.forEach(key => keydict[key.id] = { 
        id: key.id, 
        name: key.getName(), 
        typeId: key.typeReference.typeId,
        mapPath: [key.id] 
    });

    traverseInheritanceHierarchy(keydict, entity, []);

    return keydict;

    function traverseInheritanceHierarchy(keydict, curEntity, generalizationStack) {
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
                mapPath: generalizationStack.concat([key.id]) 
            };
        });
        traverseInheritanceHierarchy(keydict, nextEntity, generalizationStack);
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
})();