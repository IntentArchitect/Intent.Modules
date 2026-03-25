/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
function makeReturnTypeFileDownloadDto(element) {
    var _a;
    const commonTypes = {
        string: "d384db9c-a279-45e1-801e-e4e8099625f2",
        stream: "fd4ead8e-92e9-47c2-97a6-81d898525ea0"
    };
    let returnResultType = lookupTypesOf("DTO").find(x => x.getName() == "FileDownloadDto");
    if (!returnResultType) {
        let folderName = "Common";
        const folder = (_a = element.getPackage().getChildren("Folder").find(x => x.getName() == folderName)) !== null && _a !== void 0 ? _a : createElement("Folder", folderName, element.getPackage().id);
        returnResultType = createElement("DTO", "FileDownloadDto", folder.id);
        returnResultType.id;
        let stream = createElement("DTO-Field", "Content", returnResultType.id);
        stream.typeReference.setType(commonTypes.stream);
        let filename = createElement("DTO-Field", "Filename", returnResultType.id);
        filename.typeReference.setType(commonTypes.string);
        filename.typeReference.setIsNullable(true);
        let contentType = createElement("DTO-Field", "ContentType", returnResultType.id);
        contentType.typeReference.setType(commonTypes.string);
        contentType.typeReference.setIsNullable(true);
    }
    element.typeReference.setType(returnResultType.id);
    element.typeReference.setIsCollection(false);
    element.typeReference.setIsNullable(false);
}
function applyFileTransferStereoType(element) {
    var _a;
    const fileTransferId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";
    (_a = element.getStereotype(fileTransferId)) !== null && _a !== void 0 ? _a : element.addStereotype(fileTransferId);
}
function makePost(element) {
    var _a;
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettings = (_a = element.getStereotype(httpSettingsId)) !== null && _a !== void 0 ? _a : element.addStereotype(httpSettingsId);
    httpSettings.getProperty("Verb").setValue("POST");
}
function addUploadFields(element, childType) {
    const commonTypes = {
        string: "d384db9c-a279-45e1-801e-e4e8099625f2",
        long: "33013006-E404-48C2-AC46-24EF5A5774FD",
        stream: "fd4ead8e-92e9-47c2-97a6-81d898525ea0"
    };
    const parameterSettingId = "d01df110-1208-4af8-a913-92a49d219552";
    var existing = element.getChildren().find(x => x.getName() == "Content");
    if (!existing) {
        let stream = createElement(childType, "Content", element.id);
        stream.typeReference.setType(commonTypes.stream);
    }
    var existing = element.getChildren().find(x => x.getName() == "Filename");
    if (!existing) {
        let filename = createElement(childType, "Filename", element.id);
        filename.typeReference.setType(commonTypes.string);
        filename.typeReference.setIsNullable(true);
    }
    var existing = element.getChildren().find(x => x.getName() == "ContentType");
    if (!existing) {
        let contentType = createElement(childType, "ContentType", element.id);
        contentType.typeReference.setType(commonTypes.string);
        contentType.typeReference.setIsNullable(true);
        let parameterSetting = contentType.addStereotype(parameterSettingId);
        parameterSetting.getProperty("Source").setValue("From Header");
        parameterSetting.getProperty("Header Name").setValue("Content-Type");
    }
    var existing = element.getChildren().find(x => x.getName() == "ContentLength");
    if (!existing) {
        let contentType = createElement(childType, "ContentLength", element.id);
        contentType.typeReference.setType(commonTypes.long);
        contentType.typeReference.setIsNullable(true);
        let parameterSetting = contentType.addStereotype(parameterSettingId);
        parameterSetting.getProperty("Source").setValue("From Header");
        parameterSetting.getProperty("Header Name").setValue("Content-Length");
    }
}
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
function getSurrogateKeyType() {
    var _a, _b, _c;
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
    let typeName = (_c = (_b = (_a = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Type")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "int";
    if (typeNameToIdMap.has(typeName)) {
        return typeNameToIdMap.get(typeName);
    }
    return typeNameToIdMap.get("guid");
}
/// <reference path="../../typings/elementmacro.context.api.d.ts"/>
/// <reference path="getSurrogateKeyType.ts"/>
/// <reference path="attributeWithMapPath.ts"/>
class DomainHelper {
    static isAggregateRoot(element) {
        let result = !element.getAssociations("Association")
            .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
        return result;
    }
    static getCommandOperations(entity) {
        const queryOperationNames = ["Get", "Find", "Filter", "Query", "Is", "Must", "Can"];
        const operations = entity.getChildren("Operation").filter(operation => operation.typeReference.getType() == null ||
            !queryOperationNames.some(allowedOperationName => operation.getName().startsWith(allowedOperationName)));
        return operations;
    }
    static isComplexType(element) {
        return (element === null || element === void 0 ? void 0 : element.specialization) === "Data Contract" ||
            (element === null || element === void 0 ? void 0 : element.specialization) === "Value Object" ||
            (element === null || element === void 0 ? void 0 : element.specialization) === "Class";
    }
    static isComplexTypeById(typeId) {
        let element = lookup(typeId);
        return DomainHelper.isComplexType(element);
    }
    static getOwningAggregate(entity) {
        var _a;
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        let invalidAssociations = entity.getAssociations("Association").filter(x => x.typeReference.getType() == null);
        if (invalidAssociations.length > 0) {
            console.warn("Invalid associations found:");
            invalidAssociations.forEach(x => {
                console.warn("Invalid associations: " + x.getName());
            });
        }
        let result = (_a = entity.getAssociations("Association")
            .filter(x => this.isAggregateRoot(x.typeReference.getType()) && isOwnedBy(x) &&
            // Let's only target collections for now as part of the nested compositional crud support
            // as one-to-one relationships are more expensive to address and possibly not going to
            // be needed.
            x.getOtherEnd().typeReference.isCollection)[0]) === null || _a === void 0 ? void 0 : _a.typeReference.getType();
        return result;
        function isOwnedBy(association) {
            return association.isSourceEnd() &&
                !association.typeReference.isNullable &&
                !association.typeReference.isCollection;
        }
    }
    static ownerIsAggregateRoot(entity) {
        let result = DomainHelper.getOwningAggregate(entity);
        return result ? true : false;
    }
    static hasPrimaryKey(entity) {
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        return keys.length > 0;
    }
    static getPrimaryKeys(entity) {
        if (!entity) {
            throw new Error("entity not specified");
        }
        let primaryKeys = DomainHelper.getPrimaryKeysMap(entity);
        return Object.values(primaryKeys);
    }
    static isUserSuppliedPrimaryKey(pk) {
        if (pk == null)
            return false;
        if (!pk.hasStereotype("Primary Key"))
            return false;
        var pkStereotype = pk.getStereotype("Primary Key");
        if (!pkStereotype.hasProperty("Data source")) {
            return false;
        }
        return pkStereotype.getProperty("Data source").value == "User supplied";
    }
    static getPrimaryKeysMap(entity) {
        let keydict = Object.create(null);
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        keys.forEach(key => keydict[key.id] = {
            id: key.id,
            name: key.getName(),
            typeId: key.typeReference.typeId,
            typeReferenceModel: key.typeReference.toModel(),
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, [], new Set());
        return keydict;
        function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack, visited) {
            if (!curEntity || visited.has(curEntity.id)) {
                return;
            }
            visited.add(curEntity.id);
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
                    typeReferenceModel: key.typeReference.toModel(),
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack, visited);
        }
    }
    static getForeignKeys(entity, owningAggregate) {
        var _a;
        if (!entity) {
            throw new Error("entity not specified");
        }
        if (!owningAggregate) {
            throw new Error("nestedCompOwner not specified");
        }
        // Use the new Associated property on the FK stereotype method for FK Attribute lookup
        let foreignKeys = [];
        for (let attr of entity.getChildren("Attribute").filter(x => x.hasStereotype("Foreign Key"))) {
            let associationId = (_a = attr.getStereotype("Foreign Key").getProperty("Association")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (owningAggregate.getAssociations("Association").some(x => x.id == associationId)) {
                foreignKeys.push(attr);
            }
        }
        // Backward compatible lookup method
        if (foreignKeys.length == 0) {
            let foundFk = entity.getChildren("Attribute")
                .filter(x => x.getName().toLowerCase().indexOf(owningAggregate.getName().toLowerCase()) >= 0 && x.hasStereotype("Foreign Key"))[0];
            if (foundFk) {
                foreignKeys.push(foundFk);
            }
        }
        return foreignKeys.map(x => ({
            name: DomainHelper.getAttributeNameFormat(x.getName()),
            typeId: x.typeReference.typeId,
            typeReferenceModel: x.typeReference.toModel(),
            id: x.id,
            mapPath: [x.id],
            isCollection: x.typeReference.isCollection,
            isNullable: x.typeReference.isNullable,
            element: x
        }));
    }
    /**
     * Returns true if the attribute is a foreign key on a compositional one-to-many relationship (i.e. is managed by the DB and should not be set).
     * @param attribute
     * @returns
     */
    static isManagedForeignKey(attribute) {
        var _a, _b;
        let fkAssociation = (_b = (_a = attribute.getStereotype("Foreign Key")) === null || _a === void 0 ? void 0 : _a.getProperty("Association")) === null || _b === void 0 ? void 0 : _b.getSelected();
        return fkAssociation != null && !fkAssociation.getOtherEnd().typeReference.getIsCollection() && !fkAssociation.getOtherEnd().typeReference.getIsNullable();
    }
    static getChildrenOfType(entity, type) {
        let attrDict = Object.create(null);
        let attributes = entity.getChildren(type);
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
            isNullable: attr.typeReference.isNullable,
            isCollection: attr.typeReference.isCollection
        });
        return Object.values(attrDict);
    }
    static getAttributesWithMapPath(entity) {
        let attrDict = Object.create(null);
        let attributes = entity
            .getChildren("Attribute")
            .filter(x => {
            var _a;
            return !x.hasStereotype("Primary Key") &&
                !DomainHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || ((_a = x.getMetadata("set-by-infrastructure")) === null || _a === void 0 ? void 0 : _a.toLocaleLowerCase()) != "true"));
        });
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForAttributes(attrDict, entity, [], new Set());
        return Object.values(attrDict);
        function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack, visited) {
            if (!curEntity || visited.has(curEntity.id)) {
                return;
            }
            visited.add(curEntity.id);
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !DomainHelper.legacyPartitionKey(x));
            baseKeys.forEach(attr => {
                attrDict[attr.id] = {
                    id: attr.id,
                    name: attr.getName(),
                    typeId: attr.typeReference.typeId,
                    typeReferenceModel: attr.typeReference.toModel(),
                    mapPath: generalizationStack.concat([attr.id]),
                    isNullable: attr.typeReference.isNullable,
                    isCollection: attr.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack, visited);
        }
    }
    static getMandatoryAssociationsWithMapPath(entity) {
        return traverseInheritanceHierarchy(entity, [], [], new Set());
        function traverseInheritanceHierarchy(entity, results, generalizationStack, visited) {
            if (!entity || visited.has(entity.id)) {
                return results;
            }
            visited.add(entity.id);
            entity
                .getAssociations("Association")
                .filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable && x.typeReference.isNavigable &&
                !x.getOtherEnd().typeReference.isCollection && !x.getOtherEnd().typeReference.isNullable)
                .forEach(association => {
                return results.push({
                    id: association.id,
                    name: association.getName(),
                    typeId: null,
                    typeReferenceModel: null,
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
            return traverseInheritanceHierarchy(generalization.typeReference.getType(), results, generalizationStack, visited);
        }
    }
    static getAttributeNameFormat(str) {
        let convention = DomainHelper.getDomainAttributeNamingConvention();
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
        }
        return str;
    }
    static getDomainAttributeNamingConvention() {
        var _a, _b, _c;
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return (_c = (_b = (_a = application.getSettings(domainSettingsId)) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
    }
    static getSurrogateKeyType() {
        return getSurrogateKeyType();
    }
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
    static requiresForeignKey(associationEnd) {
        return DomainHelper.isManyToVariantsOfOne(associationEnd) || DomainHelper.isSelfReferencingZeroToOne(associationEnd);
    }
    static isManyToVariantsOfOne(associationEnd) {
        return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
    }
    static isSelfReferencingZeroToOne(associationEnd) {
        return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
    }
    static getOwningAggregateRecursive(entity) {
        let owners = DomainHelper.getOwnersRecursive(entity);
        if (owners.length == 0)
            return null;
        const uniqueIds = new Set(owners.map(item => item.id));
        if (uniqueIds.size !== 1) {
            throw new Error(`Entity : '${entity.getName()}' has more than 1 owner.`);
        }
        return owners[0];
    }
    static getOwnersRecursive(entity, visited = new Set()) {
        if (!entity || entity.specialization != "Class" || visited.has(entity.id)) {
            return [];
        }
        visited.add(entity.id);
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
        let result = [];
        for (let i = 0; i < results.length; i++) {
            let owner = results[i].typeReference.getType();
            if (DomainHelper.isAggregateRoot(owner)) {
                result.push(owner);
            }
            else {
                result.push(...DomainHelper.getOwnersRecursive(owner, visited));
            }
        }
        return result;
    }
    static isOwnedByAssociation(association) {
        return association.isSourceEnd() &&
            !association.typeReference.isNullable &&
            !association.typeReference.isCollection;
    }
    static getOwningAggregateKeyChain(entity, visited = new Set()) {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        if (visited.has(entity.id)) {
            return [];
        }
        visited.add(entity.id);
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
        let result = [];
        if (results.length == 0)
            return result;
        let owner = results[0].typeReference.getType();
        let pks = DomainHelper.getPrimaryKeys(owner);
        pks.forEach(pk => {
            let attribute = lookup(pk.id);
            //expectedName would typically be CountryId if you have a Agg: Country with a Pk: Id
            let expectedName = attribute.getParent().getName();
            if (!attribute.getName().startsWith(expectedName)) {
                expectedName += attribute.getName();
            }
            else {
                expectedName = attribute.getName();
            }
            result.push({ attribute: attribute, expectedName: expectedName });
        });
        if (!DomainHelper.isAggregateRoot(owner)) {
            result.unshift(...DomainHelper.getOwningAggregateKeyChain(owner, visited));
        }
        return result;
    }
}
/// <reference path="domainHelper.ts" />
class EntityDomainElementDetails {
    constructor(entity) {
        this.entity = entity;
        this.owningEntity = DomainHelper.getOwningAggregateRecursive(entity);
    }
    getOwningOrTargetEntityName() {
        var _a;
        return ((_a = this.owningEntity) !== null && _a !== void 0 ? _a : this.entity).getName();
    }
    hasOwningEntity() {
        return this.owningEntity != null;
    }
}
class MappedDomainElement {
    constructor(originalElement) {
        this.originalElement = originalElement;
        this.entityDomainElementDetails = this.isEntityDomainElement() ? new EntityDomainElementDetails(originalElement) : null;
    }
    isEntityDomainElement() {
        return this.originalElement.specialization == "Class";
    }
    getId() {
        return this.originalElement.id;
    }
    getName() {
        return this.originalElement.getName();
    }
}
/// <reference path="attributeWithMapPath.ts" />
class ServicesConstants {
}
ServicesConstants.dtoToEntityMappingId = "942eae46-49f1-450e-9274-a92d40ac35fa"; //"01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
ServicesConstants.dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
ServicesConstants.dtoToDomainOperation = "8d1f6a8a-77c8-43a2-8e60-421559725419";
class ServicesHelper {
    static addDtoFieldsFromDomain(dto, attributes) {
        var _a;
        for (let key of attributes) {
            if (dto && !dto.getChildren("DTO-Field").some(x => x.getName() == ServicesHelper.getFieldFormat(key.name))) {
                let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(key.name), dto.id);
                field.typeReference.setType(key.typeId);
                if (((_a = key.mapPath) !== null && _a !== void 0 ? _a : []).length > 0) {
                    field.setMapping(key.mapPath);
                }
            }
        }
    }
    static getParameterFormat(str) {
        return toCamelCase(str);
    }
    static getRoutingFormat(str) {
        return pluralize(str);
    }
    static getFieldFormat(str) {
        return toPascalCase(str);
    }
    static formatName(str, type) {
        switch (type) {
            case "property":
            case "class":
                return toPascalCase(str);
            case "parameter":
                return toCamelCase(str);
            default:
                return str;
        }
    }
}
/**
 * Workaround for element's from referenced packages not having getParent()
 * @param element The element whose parent should be searched for
 * @param parentSpecializationType The specialization type of the parent
 */
function getParent(element, parentSpecializationType) {
    const elements = lookupTypesOf(parentSpecializationType);
    const parent = elements
        .find(x => x.getChildren(element.specialization)
        .some(child => child.id === element.id));
    if (parent == null) {
        throw new Error(`Could not find parent for ${element.id}, ${element.getName()}, ${element.specialization}`);
    }
    return parent;
}
/// <reference path="../common/domainHelper.ts" />
/// <reference path="../common/servicesHelper.ts" />
/// <reference path="../common/getParent.ts" />
/**
 * Gets select details of a mapped Command/Query. Intended for centralized logic of working out
 * things like keys for both the entity and owning entity if applicable.
 *
 * If the Command is for entity creation (either due to being mapped to a constructor or being
 * prefixed with "Create"), then primary keys for the entity are not populated.
 * @param request The Command or Query that has been mapped
 */
function getMappedRequestDetails(request) {
    var _a, _b;
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
    const updateEntityMappingTypeId = "01721b1a-a85d-4320-a5cd-8bd39247196a";
    // Basic mapping:
    let mappedElement = (_a = request.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement();
    // Advanced mapping:
    if (mappedElement == null) {
        const advancedMappings = request.getAssociations()
            .map((association) => {
            return association.getMapping(createEntityMappingTypeId) ||
                association.getMapping(queryEntityMappingTypeId) ||
                association.getMapping(updateEntityMappingTypeId);
        })
            .filter(mapping => mapping != null);
        if (advancedMappings.length === 1) {
            mappedElement = (_b = advancedMappings[0].getMappedEnds()[0]) === null || _b === void 0 ? void 0 : _b.getTargetElement();
        }
    }
    if (mappedElement == null) {
        return null;
    }
    let entity = mappedElement;
    if (entity.specialization !== "Class") {
        entity = getParent(entity, "Class");
    }
    const result = {
        entity: entity,
        mappingElement: mappedElement,
        mappingTargetType: mappedElement.specialization,
        entityKeyFields: [],
        ownerKeyFields: []
    };
    result.owningEntity = DomainHelper.getOwningAggregate(entity);
    // As long as it's not for creation, populate the PKs of the entity:
    if (result.mappingTargetType !== "Class Constructor" &&
        !request.getName().toLowerCase().startsWith("Create")) {
        result.entityKeyFields = result.mappingTargetType === "Class"
            ? getKeysForClassMapping(request, entity)
            : getKeysForOperationMapping(request, entity);
    }
    // If the entity is owned, populate its fields:
    if (result.owningEntity != null) {
        result.ownerKeyFields = result.mappingTargetType === "Class"
            ? getKeysForClassMapping(request, entity, result.owningEntity)
            : getKeysForOperationMapping(request, entity, result.owningEntity);
    }
    return result;
    /**
     * Return field details for primary keys. As requests mapped to operations and constructors can
     * never possibly map the attributes, these fields can only ever be matched by name.
     * @param request The CQRS Command or Query entity
     * @param owningEntity The Owning Aggregate Class
     */
    function getKeysForOperationMapping(request, entity, owningEntity) {
        const pks = DomainHelper.getPrimaryKeys(owningEntity !== null && owningEntity !== void 0 ? owningEntity : entity);
        return pks.map(pk => {
            let fieldName = toPascalCase(pk.name);
            if (owningEntity != null) {
                fieldName = removePrefix(fieldName, toPascalCase(owningEntity.getName()));
                fieldName = `${owningEntity.getName()}${toCamelCase(fieldName)}`;
            }
            fieldName = ServicesHelper.getFieldFormat(fieldName);
            const existingField = request.getChildren("DTO-Field")
                .find(field => field.getName().toLowerCase() == fieldName.toLowerCase() || field.getName().toLowerCase() == "id");
            return {
                existingId: existingField === null || existingField === void 0 ? void 0 : existingField.id,
                mappingPath: [],
                name: fieldName,
                typeId: pk.typeId,
                isCollection: pk.isCollection,
                isNullable: pk.isNullable
            };
        });
    }
    function getKeysForClassMapping(request, entity, owningEntity) {
        const keys = owningEntity != null
            ? DomainHelper.getForeignKeys(entity, owningEntity)
            : DomainHelper.getPrimaryKeys(entity);
        return keys.map(pk => {
            const existingField = request.getChildren("DTO-Field").find(field => {
                if (field.getMapping() != null) {
                    return field.getMapping().getPath().some(x => x.id == pk.id);
                }
                return (pk.name.toLowerCase() === field.getName().toLowerCase());
            });
            return {
                existingId: existingField === null || existingField === void 0 ? void 0 : existingField.id,
                mappingPath: pk.mapPath,
                name: pk.name,
                typeId: pk.typeId,
                isCollection: pk.isCollection,
                isNullable: pk.isNullable
            };
        });
    }
}
/// <reference path="../../common/mappedDomainElement.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
function getDefaultRoutePrefix(includeLastPathSeparator) {
    const defaultApiRoutePrefix = "api/";
    const apiSettingsId = "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb";
    let settingsGroup = application.getSettings(apiSettingsId);
    let route = settingsGroup ? settingsGroup.getField("Default API Route Prefix").value : null;
    // if the group is not present, use the default value
    if (!settingsGroup) {
        route = defaultApiRoutePrefix;
    }
    // if the route is null (or set to blank in settings, which results in null)
    // set it to blank (the actual value in settings)
    if (!route || route == "") {
        return "";
    }
    if (includeLastPathSeparator && !route.endsWith("/")) {
        route += "/";
    }
    else if (!includeLastPathSeparator && route.endsWith("/")) {
        route = removeSuffix(route, "/");
    }
    return route;
}
function getFolderParts(request, domainElement) {
    var _a;
    let depth = 0;
    let currentElement = request.getParent();
    let folderParts = [];
    while (currentElement.specialization === "Folder") {
        let folderName = currentElement.getName();
        if (depth === 0 && domainElement != null) {
            const singularizedFolderName = singularize(folderName);
            const singularizedAggregateRootName = singularize((_a = domainElement.entityDomainElementDetails) === null || _a === void 0 ? void 0 : _a.getOwningOrTargetEntityName());
            if (singularizedFolderName.toLowerCase() === singularizedAggregateRootName.toLowerCase()) {
                folderName = pluralize(singularizedFolderName);
            }
        }
        folderParts.unshift(toKebabCase(folderName));
        currentElement = currentElement.getParent();
        depth++;
    }
    return folderParts;
}
function getRouteParts(request, domainElement) {
    var _a, _b;
    if (domainElement == null) {
        throw new Error("entity is required");
    }
    // all the default CQRS endpoints
    let defaultCQRSEndpoints = [`Create${domainElement.getName()}Command`, `Delete${domainElement.getName()}Command`,
        `Update${domainElement.getName()}Command`, `Get${pluralize(domainElement.getName())}Query`,
        `Get${domainElement.getName()}ByIdQuery`
    ];
    let isDefaultEndpoint = defaultCQRSEndpoints.includes(request.getName());
    // Following the RESTful naming conventions from https://restfulapi.net/resource-naming/.
    // RESTful naming conventions applied, with the exception of Operation names.
    // Operation names are developer-defined and should be left as-is to avoid potential unintended side effects.
    let routeParts = [];
    const mappedDetails = getMappedRequestDetails(request);
    if (!mappedDetails && isDefaultEndpoint) {
        if (((_a = domainElement.entityDomainElementDetails) === null || _a === void 0 ? void 0 : _a.hasOwningEntity()) == true) {
            routeParts.push(...getOwningAggregateRouting(request, domainElement));
        }
        return routeParts;
    }
    else if (!mappedDetails && !isDefaultEndpoint) {
        routeParts.push(...generateNonDefaultEndpointRouteName(request, domainElement.getName()));
        return routeParts;
    }
    // Add the owning entity's ids as parts surrounded with curly braces
    if (((_b = domainElement.entityDomainElementDetails) === null || _b === void 0 ? void 0 : _b.hasOwningEntity()) == true) {
        routeParts.push(...getOwningAggregateRouting(request, domainElement));
    }
    // Add the entity's ids as parts surrounded with curly braces
    let entIdRouteParts = [...mappedDetails.entityKeyFields
            .filter(x => x.existingId != null)
            .map(x => {
            const field = request
                .getChildren("DTO-Field")
                .find(field => field.id === x.existingId);
            return `{${toCamelCase(field.getName())}}`;
        })];
    routeParts.push(...entIdRouteParts);
    // Add the operation's name:
    if (mappedDetails.mappingTargetType === "Operation") {
        let operationName = mappedDetails.mappingElement.getName();
        routeParts.push(toKebabCase(operationName));
    }
    if (!isDefaultEndpoint && mappedDetails.mappingTargetType !== "Operation") {
        routeParts.push(...generateNonDefaultEndpointRouteName(request, domainElement.getName()));
    }
    return routeParts;
}
function getOwningAggregateRouting(request, domainElement) {
    let routeParts = [];
    let keys = DomainHelper.getOwningAggregateKeyChain(domainElement.entityDomainElementDetails.entity);
    let parentName = keys[0].attribute.getParent().getName();
    console.warn(parentName);
    keys.forEach(pk => {
        //Always add aggregate parents event if the keys are not present
        //countries/{countryId}/states/{stateId}/cities/{id} - With Keys
        //country/states/cities/{id} - Without Keys
        if (parentName != pk.attribute.getParent().getName()) {
            let newParent = toKebabCase(pluralize(pk.attribute.getParent().getName()));
            routeParts.push(newParent);
            parentName = pk.attribute.getParent().getName();
        }
        if (request.getChildren().some(x => x.getName().toLowerCase() == pk.expectedName.toLowerCase())) {
            routeParts.push(`{${toCamelCase(pk.expectedName)}}`);
        }
    });
    let ownedEntName = toKebabCase(pluralize(domainElement.getName()));
    routeParts.push(ownedEntName);
    return routeParts;
}
function generateNonDefaultEndpointRouteName(operation, domainName, additionalReplacement = []) {
    let operationName = operation.getName();
    let routeParts = [];
    // filter out some common phrases
    let toReplace = [
        `Get${pluralize(domainName)}`, `Find${pluralize(domainName)}`, `Lookup${pluralize(domainName)}`,
        `Get${domainName}`, `Find${domainName}`, `Lookup${domainName}`,
        `${domainName}`, `${pluralize(domainName)}`,
        `Query`, `Request`, `ById`, `Create`, `Update`, `Delete`, `Modify`, `Insert`, `Patch`, `Remove`, `Add`, `Set`, `List`, `Command`
    ];
    // additionalReplacement will mostly contain the folder paths. So if in the Service designer there is a Query/Command in a folder, 
    // we would want to replace that in the generated route. E.g. A query called "GetSpecialProductDataQuery", NOT linked to a domain
    // put in the product folder, instead of it generating "special-product-data", it should generate "special-data".
    // additionalReplacement will contain "product", so we supplement it with "Product", "Products" and "products"
    let supplementAdditionalReplacement = [];
    additionalReplacement.forEach((replacement) => {
        supplementAdditionalReplacement.push(pluralize(replacement[0].toUpperCase() + replacement.substring(1)));
        supplementAdditionalReplacement.push(pluralize(replacement));
        supplementAdditionalReplacement.push(replacement[0].toUpperCase() + replacement.substring(1));
        supplementAdditionalReplacement.push(singularize(replacement[0].toUpperCase() + replacement.substring(1)));
        supplementAdditionalReplacement.push(singularize(replacement));
    });
    toReplace.push(...supplementAdditionalReplacement);
    toReplace.push(...additionalReplacement);
    // sort longest to shortest
    toReplace.sort((a, b) => b.length - a.length).forEach((search) => {
        operationName = operationName.replace(search, '');
    });
    // convert to kebab case, and then correct it based on acronyms (e.g. SMS)
    let cleanedOperationName = kebabCaseAcronymCorrection(toKebabCase(operationName), operationName);
    if (cleanedOperationName) {
        routeParts.push(cleanedOperationName);
    }
    return routeParts;
}
function getEntityInheritanceHierarchyIds(curEntity) {
    let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
    if (generalizations.length == 0) {
        return [curEntity.id];
    }
    let other = getEntityInheritanceHierarchyIds(generalizations[0].typeReference.getType());
    return other.concat(curEntity.id);
}
// this method will correct incorrect acronym/initialism in the kebab case.
// For example, "SendSMS" will become "send-s-m-s", and this method will correct it to "send-sms"
function kebabCaseAcronymCorrection(kebabInput, originalInput) {
    // Split the kebab-case result into individual parts
    let parts = kebabInput.split('-');
    let correctedParts = [];
    var currentPosition = originalInput;
    var currentWord = "";
    for (let part of parts) {
        if (part.length != 1) {
            if (currentWord != "") {
                correctedParts.push(currentWord);
                currentWord = "";
            }
            correctedParts.push(part);
            currentPosition = currentPosition.substring(part.length);
            continue;
        }
        if (currentPosition.startsWith(part.toUpperCase())) {
            currentWord = currentWord + part;
            currentPosition = currentPosition.substring(part.length);
        }
    }
    if (currentWord != "") {
        correctedParts.push(currentWord);
        currentWord = "";
    }
    // Join the corrected parts back into a kebab-case string
    return correctedParts.join('-');
}
/// <reference path="mappedDomainElement.ts" />
/**
 * Gets the ultimate target entity and it's owning entity (if it has one) of a mapped Command/Query.
 * @param request The Command or Query that has been mapped
 */
function getMappedDomainElement(request) {
    var _a;
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
    const mappingTypeIds = [queryEntityMappingTypeId, createEntityMappingTypeId];
    const mappableElements = ["Class", "Repository"];
    const isMappableElement = function (element) {
        return mappableElements.some(x => (element === null || element === void 0 ? void 0 : element.specialization) === x);
    };
    let entity = null;
    // Basic mapping:
    let mappedElement = (_a = request.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement();
    if (mappedElement != null) {
        let element = mappedElement;
        while (element != null) {
            if (isMappableElement(element)) {
                entity = element;
                break;
            }
            element = element.getParent();
        }
    }
    // Advanced mappings:
    if (mappedElement == null) {
        const targetEntities = request.getAssociations()
            .flatMap((association) => association.getMappings()
            .filter(mapping => mappingTypeIds.some(y => mapping.mappingTypeId == y))
            .map(mapping => {
            let element = mapping.getTargetElement();
            while (element != null) {
                if (isMappableElement(element)) {
                    return element;
                }
                element = element.getParent();
            }
            return null;
        })
            .filter(entity => entity != null));
        // Only if all the targetClasses are the same:
        if (targetEntities.length > 0 && targetEntities.every(x => x.id === targetEntities[0].id)) {
            entity = targetEntities[0];
        }
    }
    if (entity == null) {
        return null;
    }
    return new MappedDomainElement(entity);
}
/// <reference path="../../common/getMappedDomainElement.ts" />
function applyHttpSettingsToOperations(operation, existingRoute = ``) {
    var _a, _b;
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6"; // from WebApi module
    const parameterSettingsId = "d01df110-1208-4af8-a913-92a49d219552"; // from WebApi module
    const httpSettingsMediatypeId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8";
    if (!operation.hasStereotype(httpSettingsId)) {
        operation.addStereotype(httpSettingsId);
    }
    // get the name of the service, based on auto CRUD creation convention
    let serviceDomain = ``;
    if (operation.getParent() != null) {
        var serviceName = operation.getParent().getName();
        serviceDomain = singularize(serviceName.replace(`Service`, ``));
    }
    const domainElement = getMappedDomainElement(operation);
    // filter out some common phrases
    let toReplace = [
        `Query`, `Request`, `ById`, `Create`, `Update`, `Delete`, `Modify`, `Insert`, `Patch`, `Remove`,
        `Add`, `Set`, `List`, `Command`, `Find`, `Get`
    ];
    let supplementAdditionalReplacement = [];
    existingRoute.split('/').forEach((replacement) => {
        if (replacement.length > 0) {
            supplementAdditionalReplacement.push(replacement[0].toUpperCase() + replacement.substring(1));
            supplementAdditionalReplacement.push(pluralize(replacement[0].toUpperCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(pluralize(replacement[0].toLowerCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(pluralize(replacement));
            supplementAdditionalReplacement.push(singularize(replacement[0].toUpperCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(singularize(replacement[0].toLowerCase() + replacement.substring(1)));
            supplementAdditionalReplacement.push(singularize(replacement));
        }
    });
    toReplace.push(...supplementAdditionalReplacement);
    let operationName = operation.getName();
    toReplace.sort((a, b) => b.length - a.length).forEach((search) => {
        operationName = operationName.replace(search, '');
    });
    let routePrefix = "";
    if (domainElement != null && domainElement.entityDomainElementDetails.hasOwningEntity()) {
        let routes = getOwningAggregateRouting(operation, domainElement);
        routePrefix = routes.join("/");
        serviceDomain = singularize(domainElement.entityDomainElementDetails.entity.getName());
    }
    let entity = (_a = domainElement === null || domainElement === void 0 ? void 0 : domainElement.entityDomainElementDetails) === null || _a === void 0 ? void 0 : _a.entity;
    // first check if its the standard default operations
    // if its not one of the "defaults" setup by the CRUD accelerator
    // then calculate the route
    const httpSettings = operation.getStereotype(httpSettingsId);
    if (operation.getName() === `Create${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, false, entity));
    }
    else if (operation.getName() === `Update${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("PUT");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, true, entity));
    }
    else if (operation.getName() === `Delete${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("DELETE");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, true, entity));
    }
    else if (operation.getName() === `Patch${serviceDomain}`) {
        httpSettings.getProperty("Verb").setValue("PATCH");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, true, entity));
    }
    else if (operation.getName() === `Find${serviceDomain}ById`) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, true, entity));
    }
    else if (operation.getName() === `Find${pluralize(serviceDomain)}`) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, false, entity));
    }
    else if (_isMappedDomainOperation(operation)) {
        httpSettings.getProperty("Verb").setValue("PUT");
        httpSettings.getProperty("Route").setValue(_getRouteInfo(operation, routePrefix, true, entity, kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)));
    }
    else if (operation.getName().startsWith("Get") || operation.getName().startsWith("Find") || operation.getName().startsWith("Lookup")) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`);
    }
    else if (operation.typeReference.getType() != null) {
        httpSettings.getProperty("Verb").setValue("GET");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}${(operation.getChildren().some(x => x.getName().toLowerCase() == "id") ? `/{id}` : "")}`);
    }
    else {
        httpSettings.getProperty("Verb").setValue("POST");
        httpSettings.getProperty("Route").setValue(`${kebabCaseAcronymCorrection(toKebabCase(operationName), operationName)}`);
    }
    operation.getChildren("Parameter").forEach(parameter => {
        if (!parameter.hasStereotype(parameterSettingsId)) {
            parameter.addStereotype(parameterSettingsId);
        }
    });
    if (((_b = operation.typeReference.getType()) === null || _b === void 0 ? void 0 : _b.specialization) == "Type-Definition") {
        httpSettings.getProperty(httpSettingsMediatypeId).setValue("application/json");
    }
}
function _isMappedDomainOperation(operation) {
    var mappings = getMappedRequestDetails(operation);
    if (mappings == null)
        return false;
    return mappings.mappingTargetType === "Operation";
}
function _getRouteInfo(operation, routePrefix, addId, entity, additionalRoute) {
    let result = routePrefix;
    if (addId == true) {
        let routeIds = [];
        if (entity == null) {
            if (operation.getChildren().some(x => x.getName().toLowerCase() == "id")) {
                routeIds.push(`{id}`);
            }
        }
        else {
            let primaryKeys = DomainHelper.getPrimaryKeys(entity);
            for (const key of primaryKeys) {
                if (operation.getChildren().some(x => x.getName().toLowerCase() == key.name.toLowerCase() || x.getName().toLowerCase() == "id")) {
                    routeIds.push(`{${operation.getChildren().find(x => x.getName().toLowerCase() == key.name.toLowerCase() || x.getName().toLowerCase() == "id").getName()}}`);
                }
            }
        }
        if (result.length > 0) {
            result += "/";
        }
        result += routeIds.join("/");
    }
    if (additionalRoute != null) {
        if (result.length > 0) {
            result += "/";
        }
        result += additionalRoute;
    }
    return result;
}
/// <reference path="../_common/common.ts" />
/// <reference path="common.ts" />
function exposeOperationAsHttpEndPoint(element) {
    let parentRoute = _getParentRoute(element);
    applyHttpSettingsToOperations(element, parentRoute);
}
function exposeOperationAsHttpPatchEndpoint(element) {
    let parentRoute = _getParentRoute(element);
    applyHttpSettingsToOperations(element, parentRoute);
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    element.getStereotype(httpSettingsId).getProperty("Verb").setValue("PATCH");
}
function _getParentRoute(element) {
    let httpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd"; // from WebApi module
    if (!element.getParent().hasStereotype(httpServiceSettingsId)) {
        element.getParent().addStereotype(httpServiceSettingsId);
        let serviceBaseName = removeSuffix(element.getParent().getName(), "Service");
        element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").setValue(_getOperationRoute(serviceBaseName));
    }
    let parentRoute = element.getParent().getStereotype(httpServiceSettingsId).getProperty("Route").getValue().toString();
    return parentRoute;
}
function _getOperationRoute(serviceBaseName) {
    return `${getDefaultRoutePrefix(true)}${kebabCaseAcronymCorrection(toKebabCase(serviceBaseName), serviceBaseName)}`;
}
/// <reference path="../common/common-file-transfer.ts" />
/// <reference path="../../services-expose-as-http-endpoint/services/operation-expose-as-http-endpoint.ts" />
/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/expose-as-file-upload-operation/expose-as-file-upload-operation.ts
 */
function configureUpload(element) {
    applyFileTransferStereoType(element);
    addUploadFields(element, "Parameter");
    exposeOperationAsHttpEndPoint(element);
    makePost(element);
}
