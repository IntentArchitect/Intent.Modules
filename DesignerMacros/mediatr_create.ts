declare var globals;
(async () => {

var globals = initGlobals();

// Detect if we're running from the "Execution Script dialogue", in which case we want to manually
// set the element to something:
let defaultDomainClassName = null;
if (element?.id == null) {
    // For testing as if a package was right clicked, substitute with package id:
    //element = getPackages().filter(x => x.id == "eafc12ee-88c0-4e8e-abcf-2b989be5d656")[0];

    // For element, substitute with service's element id:
    // element = lookup("677c6801-e654-45c5-924e-886713db1f5e");

    // When set, the dialog asking to select the class from the domain is skipped:
    //defaultDomainClassName = "NewClass";
}



let entity = await preselectOrPromptEntity(defaultDomainClassName);
if (!entity) { return; }
let service = getServiceFromCurrentContext(entity, element);

let entityFolder = getEntityFolder(service, entity);
let resultCqrsTypeDto = createCqrsResultTypeDto(entity, entityFolder);

createCqrsCreateOperation(service, entityFolder, entity);
createCqrsFindByIdOperation(service, entity, entityFolder, resultCqrsTypeDto);
createCqrsFindAllOperation(service, entity, entityFolder, resultCqrsTypeDto);
createCqrsUpdateOperation(service, entity, entityFolder);
createCqrsDeleteOperation(service, entity, entityFolder);

/*
========================
   HOOK-IN FUNCTIONS
========================
Easier to alter the behavior of certain key operations.
Could make certain things configurable in the future.
*/

function getParameterFormat(str) : string {
    return toCamelCase(str);
}

function getRoutingFormat(str) : string {
    return pluralize(str);
}

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

function getFolderName(nestedCompOwner, entity) : string {
    return nestedCompOwner ? pluralize(nestedCompOwner.name) : pluralize(entity.name);
}

function getBaseNameForElement(nestedCompOwner, entity, entityIsMany) : string {
    let entityName = entityIsMany ? toPascalCase(pluralize(entity.name)) : toPascalCase(entity.name);
    return nestedCompOwner ? `${toPascalCase(nestedCompOwner.name)}${entityName}` : entityName;
}

function getServiceName(nestedCompOwner, entity) : string {
    return nestedCompOwner 
        ? `${toPascalCase(pluralize(nestedCompOwner.name))}Service` 
        : `${toPascalCase(pluralize(entity.name))}Service`;
}

function getOperationFormat(baseName, nestedCompOwner, entity, entityIsMany = false) : string {
    let entityName = entityIsMany ? pluralize(entity.name) : entity.name;
    return `${baseName}${nestedCompOwner ? entityName : ""}`;
}

/*
========================
   CREATION FUNCTIONS
========================
*/

function createCqrsResultTypeDto(entity, entityFolder) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, false);
    let expectedDtoName = `${baseName}Dto`;

    if (hasElementInFolder(entityFolder, expectedDtoName)) {
        return entityFolder.getChildren().filter(x => x.name == expectedDtoName)[0];
    }

    let dto = createElement("DTO", expectedDtoName, entityFolder.id);
    dto.setMapping(entity.id);
    dto.setMetadata("baseName", baseName);

    let entityPkDescr = getPrimaryKeyDescriptor(entity);

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);

        if (!nestedCompOwnerFkDescr || nestedCompOwnerFkDescr.specialization == globals.FKSpecialization.Implicit) {
            let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), dto.id);
            nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        }
    }

    if (entityPkDescr.specialization == globals.PKSpecialization.Implicit) {
        let idField = createElement("DTO-Field", getFieldFormat(entityPkDescr.name), dto.id);
        idField.typeReference.setType(entityPkDescr.typeId);
    }

    addPrimaryKeysCqrs(dto, null, entityPkDescr);

    let attributesWithMapPaths = getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { continue; }
        let field = createElement("DTO-Field", getFieldFormat(entry.name), dto.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
    }

    dto.collapse();
    return dto;
}

function createCqrsCreateOperation(service, entityFolder, entity) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, false);
    let expectedCommandName = `Create${baseName}Command`;
    
    if (hasElementInFolder(entityFolder, expectedCommandName)) {
        let command = entityFolder.getChildren().filter(x => x.name == expectedCommandName)[0];
        let entityPkDescr = getPrimaryKeyDescriptor(entity);
        command.typeReference.setType(entityPkDescr.typeId);
        return;
    }

    let command = createElement("Command", expectedCommandName, entityFolder.id);
    command.setMapping(entity.id);
    command.setMetadata("baseName", baseName);

    let operation = createElement("Operation", getOperationFormat("Post", nestedCompOwner, entity), service.id);
    operation.setMapping(command.id);

    let entityPkDescr = getPrimaryKeyDescriptor(entity);
    let routePath = "";

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);

        let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), command.id);
        nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId)
        if (nestedCompOwnerFkDescr.specialization == globals.FKSpecialization.Explicit) {
            nestedCompOwnerIdDtoField.setMapping(nestedCompOwnerFkDescr.id);
        }

        let param = createElement("Parameter", getParameterFormat(nestedCompOwnerFkDescr.name), operation.id);
        param.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        param.setMapping(nestedCompOwnerIdDtoField.id);

        routePath = getRoutePath(nestedCompOwnerFkDescr, entity, null);
    }

    setHttpStereotype(operation, "Http Settings", {"Verb": "POST", "Route": routePath});

    let operationParamCommand = createElement("Parameter", getParameterFormat("command"), operation.id);
    operationParamCommand.typeReference.setType(command.id);

    if (entityPkDescr.typeId) {
        command.typeReference.setType(entityPkDescr.typeId);
        getReturnTypeMediatypeProperty(operation).setValue("application/json");
    }

    let attributesWithMapPaths = getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { continue; }
        let field = createElement("DTO-Field", getFieldFormat(entry.name), command.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
    }

    command.collapse();
    operation.collapse();
}

function createCqrsFindByIdOperation(service, entity, entityFolder, resultTypeDto) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, false);
    let expectedQueryName = `Get${baseName}ByIdQuery`;
    
    if (hasElementInFolder(entityFolder, expectedQueryName)) {
        return;
    }

    let query = createElement("Query", expectedQueryName, entityFolder.id);
    query.typeReference.setType(resultTypeDto.id)
    query.setMapping(entity.id);
    query.setMetadata("baseName", baseName);

    let operation = createElement("Operation", getOperationFormat("Get", nestedCompOwner, entity), service.id);
    operation.setMapping(query.id);

    let entityPkDescr = getPrimaryKeyDescriptor(entity);
    let routePath = "";

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);
        
        let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), query.id);
        nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        if (nestedCompOwnerFkDescr.specialization == globals.FKSpecialization.Explicit) {
            nestedCompOwnerIdDtoField.setMapping(nestedCompOwnerFkDescr.id);
        }

        let param = createElement("Parameter", getParameterFormat(nestedCompOwnerFkDescr.name), operation.id);
        param.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        param.setMapping(nestedCompOwnerIdDtoField.id);

        routePath = getRoutePath(nestedCompOwnerFkDescr, entity, entityPkDescr);
    } else {
        routePath = getRoutePath(null, null, entityPkDescr);
    }

    setHttpStereotype(operation, "Http Settings", {"Verb": "GET", "Route": routePath});
    addPrimaryKeysCqrs(query, operation, entityPkDescr);

    query.collapse();
    operation.collapse();
}

function createCqrsFindAllOperation(service, entity, entityFolder, resultTypeDto) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, true);
    let expectedQueryName = `Get${baseName}Query`;
    
    if (hasElementInFolder(entityFolder, expectedQueryName)) {
        return;
    }

    let query = createElement("Query", expectedQueryName, entityFolder.id);
    query.typeReference.setType(resultTypeDto.id)
    query.typeReference.setIsCollection(true);
    query.setMetadata("baseName", baseName);

    let operation = createElement("Operation", getOperationFormat("GetAll", nestedCompOwner, entity, true), service.id);
    operation.setMapping(query.id);

    let routePath = "";

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);

        let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), query.id);
        nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId);

        let param = createElement("Parameter", getParameterFormat(nestedCompOwnerFkDescr.name), operation.id);
        param.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        param.setMapping(nestedCompOwnerIdDtoField.id);

        routePath = getRoutePath(nestedCompOwnerFkDescr, entity, null);
    }

    setHttpStereotype(operation, "Http Settings", {"Verb": "GET", "Route": routePath});

    query.collapse();
    operation.collapse();
}

function createCqrsUpdateOperation(service, entity : MacroApi.Context.IElementApi, entityFolder) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, false);
    let expectedCommandName = `Update${baseName}Command`;

    if (hasElementInFolder(entityFolder, expectedCommandName)) {
        return;
    }

    let command = createElement("Command", expectedCommandName, entityFolder.id);
    command.setMapping(entity.id);
    command.setMetadata("baseName", baseName);

    let operation = createElement("Operation", getOperationFormat("Put", nestedCompOwner, entity), service.id);
    operation.setMapping(command.id);

    let entityPkDescr = getPrimaryKeyDescriptor(entity);
    let routePath = "";

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);

        let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), command.id);
        nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId)
        if (nestedCompOwnerFkDescr.specialization == globals.FKSpecialization.Explicit) {
            nestedCompOwnerIdDtoField.setMapping(nestedCompOwnerFkDescr.id);
        }

        let param = createElement("Parameter", getParameterFormat(nestedCompOwnerFkDescr.name), operation.id);
        param.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        param.setMapping(nestedCompOwnerIdDtoField.id);

        routePath = getRoutePath(nestedCompOwnerFkDescr, entity, entityPkDescr);
    } else {
        routePath = getRoutePath(null, null, entityPkDescr);
    }

    setHttpStereotype(operation, "Http Settings", {"Verb": "PUT", "Route": routePath});
    addPrimaryKeysCqrs(command, operation, entityPkDescr);

    let commandParam = createElement("Parameter", getParameterFormat("command"), operation.id);
    commandParam.typeReference.setType(command.id);

    let attributesWithMapPaths = getAttributesWithMapPath(entity);
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (command.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == keyName)) { continue; }
        let field = createElement("DTO-Field", getFieldFormat(entry.name), command.id);
        field.typeReference.setType(entry.typeId)
        field.setMapping(entry.mapPath);
    }

    command.collapse();
    operation.collapse();
}

function createCqrsDeleteOperation(service, entity, entityFolder) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let baseName = getBaseNameForElement(nestedCompOwner, entity, false);
    let expectedCommandName = `Delete${baseName}Command`;

    if (hasElementInFolder(entityFolder, expectedCommandName)) {
        return;
    }

    let command = createElement("Command", expectedCommandName, entityFolder.id);
    command.setMapping(entity.id);
    command.setMetadata("baseName", baseName);

    let operation = createElement("Operation", getOperationFormat("Delete", nestedCompOwner, entity), service.id);
    operation.setMapping(command.id);

    let entityPkDescr = getPrimaryKeyDescriptor(entity);
    let routePath = "";

    if (nestedCompOwner) {
        let nestedCompOwnerFkDescr = getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner);

        let nestedCompOwnerIdDtoField = createElement("DTO-Field", getFieldFormat(nestedCompOwnerFkDescr.name), command.id);
        nestedCompOwnerIdDtoField.typeReference.setType(nestedCompOwnerFkDescr.typeId)
        if (nestedCompOwnerFkDescr.specialization == globals.FKSpecialization.Explicit) {
            nestedCompOwnerIdDtoField.setMapping(nestedCompOwnerFkDescr.id);
        }

        let param = createElement("Parameter", getParameterFormat(nestedCompOwnerFkDescr.name), operation.id);
        param.typeReference.setType(nestedCompOwnerFkDescr.typeId);
        param.setMapping(nestedCompOwnerIdDtoField.id);

        routePath = getRoutePath(nestedCompOwnerFkDescr, entity, entityPkDescr);
    } else {
        routePath = getRoutePath(null, null, entityPkDescr);
    }

    setHttpStereotype(operation, "Http Settings", {"Verb": "DELETE", "Route": routePath});
    addPrimaryKeysCqrs(command, operation, entityPkDescr);

    let commandParam = createElement("Parameter", getParameterFormat("command"), operation.id);
    commandParam.typeReference.setType(command.id);

    command.collapse();
    operation.collapse();
}

/*
========================
   UTILITY FUNCTIONS
========================
*/

function initGlobals() {
    return {
        aggregateRootCache: Object.create(null),
        nestedCompositionalOwnerCache: Object.create(null),
        projectMappingSettingId: "01d74d4f-e478-4fde-a2f0-9ea92255f3c5",
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

async function showObject(object) {
    await dialogService.info(JSON.stringify(object, null, "  "));
}

async function preselectOrPromptEntity(preselectedDomainClassName) {
    let classes = lookupTypesOf("Class").filter(x => isAggregateRoot(x) || x.hasStereotype("Repository") || ownerIsAggregateRoot(x));
    if (classes.length == 0) {
        await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
        return;
    }

    let classId = preselectedDomainClassName != null
        ? classes.find(x => x.getName() === preselectedDomainClassName)?.id
        : await dialogService.lookupFromOptions(classes.map((x)=>({
            id: x.id, 
            name: getFriendlyDisplayNameForClassSelection(x)
            })));
    if (classId == null) {
        await dialogService.error(`No class found with id "${classId}".`);
        return null;
    }

    let foundEntity = lookup(classId);
    return foundEntity;
}

function getFriendlyDisplayNameForClassSelection(element) {
    let found = element.getAssociations("Association").filter(x => x.isSourceEnd() && 
            !x.typeReference.isNullable && 
            !x.typeReference.isCollection && 
            isAggregateRoot(x.typeReference.getType()))[0]?.typeReference?.getType();
    return !found ? element.getName() : `${element.getName()} (${found.getName()})`;
}

function getServiceFromCurrentContext(entity, packageOrServiceElement) {
    // Auto detect if we're running in the context of a service or package.
    if (packageOrServiceElement.specialization === "Service") {
        return packageOrServiceElement;
    }

    // Must be a package. See if we can find an existing service with the name we expect.
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let expectedServiceName = getServiceName(nestedCompOwner, entity);
    let services = packageOrServiceElement.getChildren("Service").filter(x => x.name === expectedServiceName);
    if (services.length > 0) {
        return services[0];
    }

    return createElement("Service", expectedServiceName, packageOrServiceElement.id);
}

function getReturnTypeMediatypeProperty(element) {
    return element.getStereotype("Http Settings").getProperty("Return Type Mediatype");
}

function isAggregateRoot(element) {
    if (globals.aggregateRootCache[element.id]) {
        return globals.aggregateRootCache[element.id];
    }
    let result = !element.getAssociations("Association")
            .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
            globals.aggregateRootCache[element.id] = result;
    return result;
}

function getNestedCompositionalOwner(entity) {
    if (globals.nestedCompositionalOwnerCache[entity.id]) {
        return globals.nestedCompositionalOwnerCache[entity.id];
    }

    let result = entity.getAssociations("Association")
        .filter(x => isAggregateRoot(x.typeReference.getType()) &&
            isOwnedBy(x) &&
            // Let's only target collections for now as part of the nested compositional crud support
            // as one-to-one relationships are more expensive to address and possibly not going to
            // be needed.
            x.getOtherEnd().typeReference.isCollection) [0];
    if(result) {
        globals.nestedCompositionalOwnerCache[entity.id] = result.typeReference.getType();
    }
    return result;

    function isOwnedBy(association) {
        return association.isSourceEnd() && 
            !association.typeReference.isNullable && 
            !association.typeReference.isCollection;
    }
}

function ownerIsAggregateRoot(entity) {
    let result = getNestedCompositionalOwner(entity);
    return result ? true : false;
}

function getEntityFolder(service, entity) {
    let nestedCompOwner = getNestedCompositionalOwner(entity);
    let expectedFolderName = getFolderName(nestedCompOwner, entity);
    var existing = service.getParent().getChildren().find(x => x.name == expectedFolderName);
    var folder = existing || createElement("Folder", expectedFolderName, service.getParent().id);
    return folder;
}

function hasElementInFolder(entityFolder, expectedElementName) {
    return entityFolder.getChildren().some(x => x.name == expectedElementName);
}

function setHttpStereotype(element, stereotypeName, props) {
    let stereotype = element.getStereotype(stereotypeName);
    for (let key of Object.keys(props)) {
        stereotype.getProperty(key).setValue(props[key]);
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

// Returns a dictionary instead of element to help deal with explicit vs implicit keys
function getNestedCompositionalOwnerForeignKeyDescriptor(entity, nestedCompOwner) {
    if (!entity) {
        throw new Error("entity not specified");
    }
    if (!nestedCompOwner) {
        throw new Error("nestedCompOwner not specified");
    }

    let explicitFkAttr = entity.getChildren("Attribute")
        .filter(x => x.name.toLowerCase().indexOf(nestedCompOwner.name.toLowerCase()) >= 0 && x.hasStereotype("Foreign Key"))[0];
    
    if (explicitFkAttr) {
        return {
            name: getDomainAttributeNameFormat(explicitFkAttr.name),
            typeId: explicitFkAttr.typeReference.typeId,
            id: explicitFkAttr.id,
            specialization: globals.FKSpecialization.Explicit
        };
    }
    
    return {
        name: getDomainAttributeNameFormat(`${nestedCompOwner.name}Id`),
        typeId: getSurrogateKeyType(),
        id: null,
        specialization: globals.FKSpecialization.Implicit
    };
}

function addPrimaryKeysCqrs(dto : MacroApi.Context.IElementApi, operation : MacroApi.Context.IElementApi, entityPkDescr) {
    switch (entityPkDescr.specialization) {
        case globals.PKSpecialization.Implicit:
        case globals.PKSpecialization.Explicit:
            {
                if (dto) {
                    let primaryKeyDtoField = createElement("DTO-Field", getFieldFormat(entityPkDescr.name), dto.id);
                    primaryKeyDtoField.typeReference.setType(entityPkDescr.typeId);
                    if (entityPkDescr.specialization == globals.PKSpecialization.Explicit) {
                        primaryKeyDtoField.setMapping(entityPkDescr.mapPath);
                    }

                    if (operation) {
                        let operationParamId = createElement("Parameter", getParameterFormat(entityPkDescr.name), operation.id);
                        operationParamId.typeReference.setType(entityPkDescr.typeId);
                        operationParamId.setMapping(primaryKeyDtoField.id);
                    }
                }
            }
            break;
        case globals.PKSpecialization.ExplicitComposite:
            for (let key of entityPkDescr.compositeKeys) {
                if (dto) {
                    let primaryKeyDtoField = createElement("DTO-Field", getFieldFormat(key.name), dto.id);
                    primaryKeyDtoField.typeReference.setType(key.typeId)
                    primaryKeyDtoField.setMapping(key.id);

                    if (operation) {
                        let operationParamId = createElement("Parameter", getParameterFormat(key.name), operation.id);
                        operationParamId.typeReference.setType(entityPkDescr.typeId);
                        operationParamId.setMapping(primaryKeyDtoField.id);
                    }
                }
            }
            break;
    }
}

function getRoutePath(nestedCompOwnerFkDescr, entity, entityPkDescr) {
    let list = []
    if(nestedCompOwnerFkDescr) {
        list.push(`{${getParameterFormat(nestedCompOwnerFkDescr.name)}}`);
    }
    if(entity) {
        list.push(`${getRoutingFormat(entity.name)}`);
    }
    if (entityPkDescr) {
        switch (entityPkDescr.specialization) {
            case globals.PKSpecialization.Implicit:
            case globals.PKSpecialization.Explicit:
                list.push(`{${getParameterFormat(entityPkDescr.name)}}`);
                break;
            case globals.PKSpecialization.ExplicitComposite:
                list.push(entityPkDescr.compositeKeys.map(x => `{${getParameterFormat(x.name)}}`))
                break;
        }
    }
    if (list.length == 0) {
        return "";
    }
    return list.join("/");
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

