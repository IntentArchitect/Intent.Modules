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
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
        return keydict;
        function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack) {
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
                    typeReferenceModel: key.typeReference.toModel(),
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
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
        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);
        return Object.values(attrDict);
        function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack) {
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
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }
    static getMandatoryAssociationsWithMapPath(entity) {
        return traverseInheritanceHierarchy(entity, [], []);
        function traverseInheritanceHierarchy(entity, results, generalizationStack) {
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
            return traverseInheritanceHierarchy(generalization.typeReference.getType(), results, generalizationStack);
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
    static getOwnersRecursive(entity) {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
        let result = [];
        for (let i = 0; i < results.length; i++) {
            let owner = results[i].typeReference.getType();
            if (DomainHelper.isAggregateRoot(owner)) {
                result.push(owner);
            }
            else {
                result.push(...DomainHelper.getOwnersRecursive(owner));
            }
        }
        return result;
    }
    static isOwnedByAssociation(association) {
        return association.isSourceEnd() &&
            !association.typeReference.isNullable &&
            !association.typeReference.isCollection;
    }
    static getOwningAggregateKeyChain(entity) {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
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
            result.unshift(...DomainHelper.getOwningAggregateKeyChain(owner));
        }
        return result;
    }
}
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/domainHelper.ts" />
var convertToAdvancedMapping;
(function (convertToAdvancedMapping) {
    function execute() {
        if (element.isMapped() && element.specialization == "Command") {
            convertCommand(element);
        }
        else if (element.isMapped() && element.specialization == "Query") {
            convertQuery(element);
        }
    }
    convertToAdvancedMapping.execute = execute;
    function convertCommand(command) {
        var _a;
        if (!command) {
            console.warn(`Could not convert null Command.`);
            return;
        }
        if (!command.getMapping()) {
            console.warn(`Could not convert Command '${command.getName()}' without it mapping to an Entity.`);
            return;
        }
        let target = command.getMapping().getElement();
        // operation commands should always be treated as "update/put" action below
        let isOperationCommand = command.hasMetadata("isOperationCommand") && command.getMetadata("isOperationCommand") == "true";
        let entity = (_a = target.getParent("Class")) !== null && _a !== void 0 ? _a : target;
        if (command.getName().startsWith("Create") && !isOperationCommand) {
            let action = createAssociation("Create Entity Action", command.id, target.id);
            // JPS & GB: Updated the createMapping call to use createAdvancedMapping. If you are debugging
            // and this is not working, chat to JPS or GB
            let mapping = action.createAdvancedMapping(command.id, entity.id);
            mapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            mapContract("Data Mapping", command, command, [command.id], [target.id], mapping);
        }
        else if (command.getName().startsWith("Delete") && !isOperationCommand) {
            let action = createAssociation("Delete Entity Action", command.id, entity.id);
            let mapping = action.createAdvancedMapping(command.id, entity.id);
            // Query Entity Mapping
            addFilterMapping(mapping, command, entity);
            command.clearMapping();
        }
        else if (command.isMapped()) {
            let action = createAssociation("Update Entity Action", command.id, target.id);
            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(command.id, entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            addFilterMapping(queryMapping, command, entity);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(command.id, entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            if (target.id != entity.id) {
                updateMapping.addMappedEnd("Invocation Mapping", [command.id], [target.id]);
            }
            mapContract("Data Mapping", command, command, [command.id], [target.id], updateMapping);
        }
    }
    convertToAdvancedMapping.convertCommand = convertCommand;
    function addFilterMapping(mapping, command, entity) {
        var _a, _b, _c;
        let pkFields = DomainHelper.getPrimaryKeys(entity);
        if (pkFields.length == 1) {
            let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key")) || (x.getName() == "Id" || x.getName() == `${entity.getName()}Id`));
            let entityPk = pkFields[0];
            if (idField && (idField.isMapped() || entityPk)) {
                mapping.addMappedEnd("Filter Mapping", [idField.id], (_c = (_b = (_a = idField.getMapping()) === null || _a === void 0 ? void 0 : _a.getPath().map(x => x.id)) !== null && _b !== void 0 ? _b : entityPk.mapPath) !== null && _c !== void 0 ? _c : [entityPk.id]);
                idField.clearMapping();
            }
        }
        else {
            pkFields.forEach(pk => {
                var _a, _b, _c;
                let idField = command.getChildren("DTO-Field").find(x => (x.isMapped() && x.getMapping().getElement().hasStereotype("Primary Key") && x.getMapping().getElement().getName() == pk.name) || (x.getName() == pk.name));
                if (idField) {
                    mapping.addMappedEnd("Filter Mapping", [idField.id], (_c = (_b = (_a = idField.getMapping()) === null || _a === void 0 ? void 0 : _a.getPath().map(x => x.id)) !== null && _b !== void 0 ? _b : pk.mapPath) !== null && _c !== void 0 ? _c : [pk.id]);
                    idField.clearMapping();
                }
            });
        }
    }
    function convertQuery(query) {
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
        let mapping = action.createAdvancedMapping(query.id, entity.id);
        mapContract("Filter Mapping", query, query, [query.id], [entity.id], mapping);
    }
    convertToAdvancedMapping.convertQuery = convertQuery;
    function mapContract(mappingType, root, dto, sourcePath, targetPathIds, mapping) {
        if (dto.isMapped() && dto.getMapping().getElement().specialization == "Class Constructor") {
            if (targetPathIds[targetPathIds.length - 1] != dto.getMapping().getElement().id) {
                targetPathIds.push(dto.getMapping().getElement().id);
                //console.warn("Invocation Mapping : " + root.id + "->" + dto.getMapping().getElement().id);
                mapping.addMappedEnd("Invocation Mapping", [root.id], targetPathIds);
            }
        }
        dto.getChildren("DTO-Field").filter(x => x.isMapped() && !fieldsToSkip(dto, x)).forEach(field => {
            var _a, _b;
            if (((_a = field.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO" || field.typeReference.getIsCollection()) {
                //console.warn("sourcePath : " + sourcePath);
                //console.warn("targetPathIds : " + targetPathIds);    
                //console.warn("sourceAdd : " + field.id);
                //console.warn("targetAdd : " + field.getMapping().getPath().map(x => x.id));    
                mapping.addMappedEnd(mappingType, sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)));
                updateElementWithMappedElement(field);
            }
            if (((_b = field.typeReference.getType()) === null || _b === void 0 ? void 0 : _b.specialization) == "DTO") {
                mapContract(mappingType, root, field.typeReference.getType(), sourcePath.concat([field.id]), targetPathIds.concat(field.getMapping().getPath().map(x => x.id)), mapping);
            }
            field.clearMapping();
        });
        dto.clearMapping();
    }
    function fieldsToSkip(dto, field) {
        return dto.specialization == "Command" &&
            field.getMapping().getElement().hasStereotype("Primary Key") &&
            (!field.getMapping().getElement().getStereotype("Primary Key").hasProperty("Data source") || field.getMapping().getElement().getStereotype("Primary Key").getProperty("Data source").value != "User supplied");
    }
    function updateElementWithMappedElement(field) {
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
})(convertToAdvancedMapping || (convertToAdvancedMapping = {}));
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
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/**
 * Ensures that for the provided contract, it has the provided field creating it if necessary and
 * then ensure the field has the correct typeReference details and order.
 * The field's element is returned.
 */
function ensureHasField(options) {
    const { contract, fieldDetail, mappingSettingsId, order } = options;
    let field = fieldDetail.existingId != null
        ? contract.getChildren("DTO-Field").find(x => x.id === fieldDetail.existingId)
        : createElement("DTO-Field", fieldDetail.name, contract.id);
    field.typeReference.setType(fieldDetail.typeId);
    field.typeReference.setIsCollection(fieldDetail.isCollection);
    field.typeReference.setIsNullable(fieldDetail.isNullable);
    if (order != null) {
        field.setOrder(order);
    }
    if (mappingSettingsId != null) {
        field.setMapping(fieldDetail.mappingPath, mappingSettingsId);
        console.warn("mapping:" + fieldDetail.mappingPath);
    }
    return field;
}
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />
function ensureHasKeys(options) {
    const { contract, keyFields, mappingSettingsId } = options;
    let order = 0;
    for (const keyField of keyFields) {
        ensureHasField({
            contract: contract,
            fieldDetail: keyField,
            mappingSettingsId: mappingSettingsId,
            order: order++
        });
    }
}
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getSurrogateKeyType.ts" />
/// <reference path="../../common/attributeWithMapPath.ts" />
/// <reference path="../../common/domainHelper.ts" />
function getFieldFormat(str) {
    return toPascalCase(str);
}
function getDomainAttributeNameFormat(str) {
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
function getOrCreateDto(elementName, parentElement) {
    const expectedDtoName = `${elementName}Dto`;
    let existingDto = parentElement.getChildren("DTO").filter(x => x.getName() === expectedDtoName)[0];
    if (existingDto) {
        return existingDto;
    }
    let dto = createElement("DTO", expectedDtoName, parentElement.id);
    return dto;
}
function ensureDtoFieldsForCtor(autoAddPrimaryKey, ctor, dto) {
    let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");
    childrenToAdd.forEach(e => {
        if (e.mapPath != null) {
            if (dto.getChildren("Parameter").some(x => { var _a, _b; return ((_b = (_a = x.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.id) == e.id; })) {
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
function ensureDtoFields(autoAddPrimaryKey, mappedElement, dto) {
    var _a, _b;
    let dtoUpdated = false;
    let domainElement = mappedElement
        .typeReference
        .getType();
    let attributesWithMapPaths = getAttributesWithMapPath(domainElement);
    let isCreateMode = ((_b = (_a = dto.getMetadata("originalVerb")) === null || _a === void 0 ? void 0 : _a.toLowerCase()) === null || _b === void 0 ? void 0 : _b.startsWith("create")) == true;
    for (var keyName of Object.keys(attributesWithMapPaths)) {
        let entry = attributesWithMapPaths[keyName];
        if (isCreateMode && isOwnerForeignKey(entry.name, domainElement)) {
            continue;
        }
        if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
            continue;
        }
        let field = createElement("DTO-Field", toPascalCase(entry.name), dto.id);
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
function isOwnerForeignKey(attributeName, domainElement) {
    for (let association of domainElement.getAssociations().filter(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable)) {
        if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
            return true;
        }
    }
    return false;
}
function addPrimaryKeys(dto, entity, map) {
    const primaryKeys = getPrimaryKeysWithMapPath(entity);
    for (const primaryKey of primaryKeys) {
        const name = getDomainAttributeNameFormat(primaryKey.name);
        if (dto.getChildren("DTO-Field").some(x => x.getName().toLowerCase() == name.toLowerCase())) {
            continue;
        }
        const dtoField = createElement("DTO-Field", getFieldFormat(name), dto.id);
        dtoField.typeReference.setType(primaryKey.typeId);
        if (map && primaryKey.mapPath != null) {
            console.log(`Doing mapping for ${dtoField.id}`);
            dtoField.setMapping(primaryKey.mapPath);
        }
    }
}
function getPrimaryKeysWithMapPath(entity) {
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
    traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
    return Object.values(keydict);
    function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack) {
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
                typeReferenceModel: key.typeReference.toModel(),
                mapPath: generalizationStack.concat([key.id]),
                isNullable: key.typeReference.isNullable,
                isCollection: key.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
    }
}
function getAttributesWithMapPath(entity) {
    let attrDict = Object.create(null);
    let attributes = entity.getChildren("Attribute")
        .filter(x => {
        var _a;
        return !x.hasStereotype("Primary Key") &&
            !legacyPartitionKey(x) &&
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
    traverseInheritanceHierarchyForAttributes(attrDict, entity, []);
    return attrDict;
    function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack) {
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
                typeReferenceModel: attr.typeReference.toModel(),
                mapPath: generalizationStack.concat([attr.id]),
                isNullable: attr.typeReference.isNullable,
                isCollection: attr.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
    }
}
function getDomainAttributeNamingConvention() {
    var _a, _b, _c;
    const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
    return (_c = (_b = (_a = application.getSettings(domainSettingsId)) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
}
// Just in case someone still uses this convention. Used to filter out those attributes when mapping
// to domain entities that are within a Cosmos DB paradigm.
function legacyPartitionKey(attribute) {
    return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
}
/// <reference path="../common/domainHelper.ts" />
class CrudConstants {
}
CrudConstants.mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
CrudConstants.mapToDomainConstructorForDtosSettingId = "8d1f6a8a-77c8-43a2-8e60-421559725419";
CrudConstants.dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
class CrudHelper {
    // Super basic selection dialog.
    static async openBasicSelectEntityDialog(options) {
        let classes = lookupTypesOf("Class").filter(x => CrudHelper.filterClassSelection(x, options));
        if (classes.length == 0) {
            await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
            return null;
        }
        let classId = await dialogService.lookupFromOptions(classes.map((x) => ({
            id: x.id,
            name: getFriendlyDisplayNameForClassSelection(x),
            additionalInfo: `(${x.getParents().map(item => item.getName()).join("/")})`
        })));
        if (classId == null) {
            await dialogService.error(`No class found with id "${classId}".`);
            return null;
        }
        let foundEntity = lookup(classId);
        return foundEntity;
        function getFriendlyDisplayNameForClassSelection(element) {
            let aggregateEntity = DomainHelper.getOwningAggregate(element);
            return !aggregateEntity ? element.getName() : `${element.getName()} (${aggregateEntity.getName()})`;
        }
    }
    static async openCrudCreationDialog(options) {
        var _a, _b;
        let classes = lookupTypesOf("Class").filter(x => CrudHelper.filterClassSelection(x, options));
        if (classes.length == 0) {
            await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
            return null;
        }
        let dialogResult = await dialogService.openForm({
            title: "CRUD Creation Options",
            fields: [
                {
                    id: "entityId",
                    fieldType: "select",
                    label: "Entity for CRUD operations",
                    selectOptions: classes.map(x => {
                        return {
                            id: x.id,
                            description: x.getName(),
                            additionalInfo: getClassAdditionalInfo(x)
                        };
                    }),
                    isRequired: true
                },
                {
                    id: "create",
                    fieldType: "checkbox",
                    label: "Create",
                    value: "true",
                    hint: "Generate the \"Create\" operation"
                },
                {
                    id: "update",
                    fieldType: "checkbox",
                    label: "Update",
                    value: "true",
                    hint: "Generate the \"Update\" operation"
                },
                {
                    id: "queryById",
                    fieldType: "checkbox",
                    label: "Query By Id",
                    value: "true",
                    hint: "Generate the \"Query By Id\" operation"
                },
                {
                    id: "queryAll",
                    fieldType: "checkbox",
                    label: "Query All",
                    value: "true",
                    hint: "Generate the \"Query All\" operation"
                },
                {
                    id: "delete",
                    fieldType: "checkbox",
                    label: "Delete",
                    value: "true",
                    hint: "Generate the \"Delete\" operation"
                },
                {
                    id: "domain",
                    fieldType: "checkbox",
                    label: "Domain Operations",
                    value: "true",
                    hint: "Generate operations for Domain Entity operations"
                }
            ]
        });
        let foundEntity = lookup(dialogResult.entityId);
        var result = {
            selectedEntity: foundEntity,
            diagramId: dialogResult.diagramId,
            canCreate: dialogResult.create == "true",
            canUpdate: dialogResult.update == "true",
            canQueryById: dialogResult.queryById == "true",
            canQueryAll: dialogResult.queryAll == "true",
            canDelete: dialogResult.delete == "true",
            canDomain: dialogResult.domain == "true",
            selectedDomainOperationIds: []
        };
        if (result.canDomain && foundEntity.getChildren("Operation").length > 0) {
            dialogResult = await dialogService.openForm({
                title: "Select Domain Operations",
                fields: [
                    {
                        id: "tree",
                        fieldType: "tree-view",
                        label: "Domain Operations",
                        hint: "Generate operations from selected domain entity operations",
                        treeViewOptions: {
                            rootId: foundEntity.id,
                            submitFormTriggers: ["double-click", "enter"],
                            isMultiSelect: true,
                            selectableTypes: [
                                {
                                    specializationId: "Class",
                                    autoExpand: true,
                                    autoSelectChildren: false,
                                    isSelectable: (x) => false
                                },
                                {
                                    specializationId: "Operation",
                                    isSelectable: (x) => true
                                }
                            ]
                        }
                    }
                ]
            });
            result.selectedDomainOperationIds = (_b = (_a = dialogResult.tree) === null || _a === void 0 ? void 0 : _a.filter((x) => x != "0")) !== null && _b !== void 0 ? _b : [];
        }
        return result;
        function getClassAdditionalInfo(element) {
            let aggregateEntity = DomainHelper.getOwningAggregate(element);
            let prefix = aggregateEntity ? `: ${aggregateEntity.getName()}  ` : "";
            return `${prefix}(${element.getParents().map(item => item.getName()).join("/")})`;
        }
    }
    static filterClassSelection(element, options) {
        var _a;
        if (!((_a = options === null || options === void 0 ? void 0 : options.allowAbstract) !== null && _a !== void 0 ? _a : false) && element.getIsAbstract()) {
            return false;
        }
        if (element.hasStereotype("Repository")) {
            return true;
        }
        if ((options === null || options === void 0 ? void 0 : options.includeOwnedRelationships) != false && DomainHelper.ownerIsAggregateRoot(element)) {
            return DomainHelper.hasPrimaryKey(element);
        }
        if (DomainHelper.isAggregateRoot(element)) {
            let generalizations = element.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return true;
            }
            let generalization = generalizations[0];
            let parentEntity = generalization.typeReference.getType();
            //Could propagate options here but then we need to update compositional crud to support inheritance and it's already a bit of a hack
            return CrudHelper.filterClassSelection(parentEntity, { includeOwnedRelationships: false, allowAbstract: true });
        }
        return false;
    }
    static getName(command, mappedElement, dtoPrefix = null) {
        if (mappedElement.typeReference != null)
            mappedElement = mappedElement.typeReference.getType();
        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        if (dtoPrefix)
            dtoName = `${dtoPrefix}${dtoName}`;
        return dtoName;
    }
    static getOrCreateCrudDto(dtoName, mappedElement, autoAddPrimaryKey, mappingTypeSettingId, folder, inbound = false) {
        let dto = CrudHelper.getOrCreateDto(dtoName, folder);
        //dtoField.typeReference.setType(dto.id);
        const entityCtor = mappedElement
            .getChildren("Class Constructor")
            .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (inbound && entityCtor != null) {
            dto.setMapping([mappedElement.id, entityCtor.id], CrudConstants.mapToDomainConstructorForDtosSettingId);
            CrudHelper.addDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto, folder);
        }
        else {
            dto.setMapping(mappedElement.id, mappingTypeSettingId);
            CrudHelper.addDtoFields(autoAddPrimaryKey, mappedElement, dto, folder);
        }
        return dto;
    }
    static getOrCreateDto(elementName, parentElement) {
        const expectedDtoName = elementName.replace(/Dto$/, "") + "Dto";
        let existingDto = parentElement.getChildren("DTO").filter(x => x.getName() === expectedDtoName)[0];
        if (existingDto) {
            return existingDto;
        }
        let dto = createElement("DTO", expectedDtoName, parentElement.id);
        return dto;
    }
    static addDtoFieldsForCtor(autoAddPrimaryKey, ctor, dto, folder) {
        let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");
        childrenToAdd.forEach(e => {
            if (e.mapPath != null) {
                if (dto.getChildren("Parameter").some(x => { var _a, _b; return ((_b = (_a = x.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.id) == e.id; })) {
                    return;
                }
            }
            else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
            let field = createElement("DTO-Field", toPascalCase(e.name), dto.id);
            field.setMapping(e.mapPath);
            if (DomainHelper.isComplexTypeById(e.typeId)) {
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(e.typeId);
            }
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
        dto.collapse();
    }
    static addDtoFields(autoAddPrimaryKey, mappedElement, dto, folder) {
        var _a, _b;
        let dtoUpdated = false;
        let domainElement = mappedElement;
        let attributesWithMapPaths = CrudHelper.getAttributesWithMapPath(domainElement);
        let isCreateMode = ((_b = (_a = dto.getMetadata("originalVerb")) === null || _a === void 0 ? void 0 : _a.toLowerCase()) === null || _b === void 0 ? void 0 : _b.startsWith("create")) == true;
        if (autoAddPrimaryKey && !isCreateMode) {
            CrudHelper.addPrimaryKeys(dto, domainElement, true);
        }
        for (var keyName of Object.keys(attributesWithMapPaths)) {
            let entry = attributesWithMapPaths[keyName];
            if (isCreateMode && CrudHelper.isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            field.setMapping(entry.mapPath);
            if (DomainHelper.isComplexTypeById(entry.typeId)) {
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(entry.typeId);
            }
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            dtoUpdated = true;
        }
        if (dtoUpdated) {
            dto.collapse();
        }
    }
    static isOwnerForeignKey(attributeName, domainElement) {
        for (let association of domainElement.getAssociations().filter(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable)) {
            if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
                return true;
            }
        }
        return false;
    }
    static addPrimaryKeys(dto, entity, map) {
        const primaryKeys = CrudHelper.getPrimaryKeysWithMapPath(entity);
        for (const primaryKey of primaryKeys) {
            const name = CrudHelper.getDomainAttributeNameFormat(primaryKey.name);
            if (dto.getChildren("DTO-Field").some(x => x.getName().toLowerCase() == name.toLowerCase())) {
                continue;
            }
            const dtoField = createElement("DTO-Field", CrudHelper.getFieldFormat(name), dto.id);
            dtoField.typeReference.setType(primaryKey.typeId);
            if (map && primaryKey.mapPath != null) {
                console.log(`Doing mapping for ${dtoField.id}`);
                dtoField.setMapping(primaryKey.mapPath);
            }
        }
    }
    static getPrimaryKeysWithMapPath(entity) {
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
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
        return Object.values(keydict);
        function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack) {
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
                    typeReferenceModel: key.typeReference.toModel(),
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
        }
    }
    static getAttributesWithMapPath(entity) {
        let attrDict = Object.create(null);
        let attributes = entity.getChildren("Attribute")
            .filter(x => {
            var _a;
            return !x.hasStereotype("Primary Key") &&
                !DomainHelper.isManagedForeignKey(x) && // essentially also an attribute set by infrastructure
                !CrudHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || ((_a = x.getMetadata("set-by-infrastructure")) === null || _a === void 0 ? void 0 : _a.toLocaleLowerCase()) != "true"));
        });
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
            // GCB - if you're seeing this change in your script, where these used to be false, you need to check.
            // I had to "fix" this so that basic mapping DTO projections worked properly (e.g. adding OrderLines to an Order DTO via basic mapping)
            isNullable: attr.typeReference.isNullable,
            isCollection: attr.typeReference.isCollection
        });
        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);
        return attrDict;
        function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack) {
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
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !CrudHelper.legacyPartitionKey(x));
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
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }
    static getFieldFormat(str) {
        return toPascalCase(str);
    }
    static getDomainAttributeNameFormat(str) {
        let convention = CrudHelper.getDomainAttributeNamingConvention();
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
            default:
                return str;
        }
    }
    static getDomainAttributeNamingConvention() {
        var _a, _b, _c;
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return (_c = (_b = (_a = application.getSettings(domainSettingsId)) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
    }
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
}
/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />
function onMapCommand(element, isForCrudScript, excludePrimaryKeys = false, inbound = false) {
    var _a, _b, _c;
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    const mappingDetails = getMappedRequestDetails(element);
    if (mappingDetails && (isForCrudScript || mappingDetails.mappingTargetType !== "Class")) {
        let order = 0;
        let keyFields = mappingDetails.ownerKeyFields;
        if (!excludePrimaryKeys) {
            keyFields = keyFields.concat(mappingDetails.entityKeyFields);
        }
        for (const keyField of keyFields) {
            ensureHasField({
                contract: element,
                fieldDetail: keyField,
                order: order++
            });
        }
    }
    if (mappingDetails &&
        mappingDetails.mappingTargetType === "Operation" &&
        DomainHelper.isComplexType((_c = (_b = (_a = element.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.typeReference) === null || _c === void 0 ? void 0 : _c.getType())) {
        let mappedElement = element.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, mappedElement), mappedElement.typeReference.getType(), false, mapFromDomainMappingSettingId, element.getParent(), false);
        setTypeRef(element, newDto, mappedElement);
    }
    const fields = element.getChildren("DTO-Field")
        .filter(x => { var _a, _b; return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO" && x.isMapped() && ((_b = x.getMapping()) === null || _b === void 0 ? void 0 : _b.getElement().specialization.startsWith("Association")); });
    fields.forEach(field => {
        let mappedElement = field.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, mappedElement), mappedElement.typeReference.getType(), !excludePrimaryKeys, projectMappingSettingId, element.getParent(), inbound);
        setTypeRef(field, newDto, mappedElement);
    });
    const complexFields = element.getChildren("DTO-Field")
        .filter(x => {
        var _a, _b, _c, _d;
        return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO" &&
            DomainHelper.isComplexType((_d = (_c = (_b = x.getMapping()) === null || _b === void 0 ? void 0 : _b.getElement()) === null || _c === void 0 ? void 0 : _c.typeReference) === null || _d === void 0 ? void 0 : _d.getType());
    });
    complexFields.forEach(cf => {
        let mappedElement = cf.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, mappedElement), mappedElement.typeReference.getType(), false, projectMappingSettingId, element.getParent(), inbound);
        setTypeRef(cf, newDto, mappedElement);
    });
    function setTypeRef(element, newDto, mappedElement) {
        var _a, _b;
        element.typeReference.setType(newDto.id);
        if (((_a = mappedElement === null || mappedElement === void 0 ? void 0 : mappedElement.typeReference) === null || _a === void 0 ? void 0 : _a.isCollection) != null) {
            element.typeReference.setIsCollection(mappedElement.typeReference.isCollection);
        }
        if ((_b = mappedElement === null || mappedElement === void 0 ? void 0 : mappedElement.typeReference) === null || _b === void 0 ? void 0 : _b.isNullable) {
            element.typeReference.setIsNullable(mappedElement.typeReference.isNullable);
        }
    }
}
/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
const stringTypeId = "d384db9c-a279-45e1-801e-e4e8099625f2";
function onMapDto(element, folder, autoAddPrimaryKey = true, dtoPrefix = null, inbound = false) {
    if (element.isMapped()) {
        let mappedFields = element.getChildren("DTO-Field").filter(x => x.getMapping());
        let unmappedFields = element.getChildren("DTO-Field").filter(x => !x.getMapping());
        for (let mappedField of mappedFields) {
            // Unfortunately have to take into account Intent's ability to disambiguate newly created fields... (the "1")
            let matchedUnmappedField = unmappedFields
                .filter(x => `${x.getName()}1` === mappedField.getName() ||
                x.getName() === mappedField.getName())[0];
            if (!matchedUnmappedField) {
                continue;
            }
            matchedUnmappedField.setMapping(mappedField.getMapping().getElement().id, mappedField.getMapping().mappingSettingsId);
            mappedField.delete();
        }
    }
    let fields = element.getChildren("DTO-Field")
        .filter(x => { var _a, _b, _c; return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO" && ((_c = (_b = x.getMapping()) === null || _b === void 0 ? void 0 : _b.getElement()) === null || _c === void 0 ? void 0 : _c.specialization.startsWith("Association")); });
    fields.forEach(f => {
        var _a, _b, _c, _d;
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        let nameArg = CrudHelper.getName(element, f.getMapping().getElement().typeReference.getType(), dtoPrefix);
        const expectedDtoName = `${nameArg.replace(/Dto$/, '')}Dto`;
        if (expectedDtoName === element.getName()) {
            const disambiguator = f.getName()
                || ((_d = (_c = (_b = (_a = f.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.typeReference) === null || _c === void 0 ? void 0 : _c.getType()) === null || _d === void 0 ? void 0 : _d.getName())
                || 'Details';
            nameArg = `${nameArg}${disambiguator}`;
        }
        const effectiveFolder = folder !== null && folder !== void 0 ? folder : element.getParent();
        let newDto = CrudHelper.getOrCreateCrudDto(nameArg, f.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, targetMappingSettingId, effectiveFolder, inbound);
        f.typeReference.setType(newDto.id);
    });
    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => {
        var _a, _b, _c, _d;
        return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO"
            && (DomainHelper.isComplexType((_d = (_c = (_b = x.getMapping()) === null || _b === void 0 ? void 0 : _b.getElement()) === null || _c === void 0 ? void 0 : _c.typeReference) === null || _d === void 0 ? void 0 : _d.getType()));
    });
    complexAttributes.forEach(f => {
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        const nameArg = CrudHelper.getName(element, f.getMapping().getElement(), dtoPrefix);
        const effectiveFolder = folder !== null && folder !== void 0 ? folder : element.getParent();
        let newDto = CrudHelper.getOrCreateCrudDto(nameArg, f.getMapping().getElement().typeReference.getType(), false, targetMappingSettingId, effectiveFolder, inbound);
        f.typeReference.setType(newDto.id);
    });
}
/// <reference path="./onMapFunctions.ts" />
function onMapQuery(element) {
    var complexTypes = ["Data Contract", "Value Object"];
    let fields = element.getChildren("DTO-Field")
        .filter(x => { var _a; return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO" && x.isMapped() && x.getMapping().getElement().specialization.startsWith("Association"); });
    fields.forEach(f => {
        getOrCreateQueryCrudDto(element, f);
    });
    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => {
        var _a, _b, _c, _d, _e;
        return ((_a = x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) != "DTO"
            && (complexTypes.includes((_e = (_d = (_c = (_b = x.getMapping()) === null || _b === void 0 ? void 0 : _b.getElement()) === null || _c === void 0 ? void 0 : _c.typeReference) === null || _d === void 0 ? void 0 : _d.getType()) === null || _e === void 0 ? void 0 : _e.specialization));
    });
    complexAttributes.forEach(f => {
        getOrCreateQueryCrudDto(element, f);
    });
}
function getOrCreateQueryCrudDto(element, dtoField) {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    let mappedElement = dtoField.getMapping().getElement();
    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName")
        ? `${element.getMetadata("baseName")}${domainName}`
        : `${domainName}`;
    let dtoName = baseName;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), projectMappingSettingId);
    dto.setMetadata("baseName", baseName);
    ensureDtoFieldsQuery(mappedElement, dto);
    dtoField.typeReference.setType(dto.id);
}
function ensureDtoFieldsQuery(mappedElement, dto) {
    let dtoUpdated = false;
    let mappedElementAttributes = mappedElement
        .typeReference
        .getType()
        .getChildren("Attribute");
    let dtoFields = dto.getChildren("DTO-Field");
    for (let attribute of mappedElementAttributes.filter(x => !dtoFields.some(y => x.getName() === y.getName()))) {
        if (dto.getChildren("DTO-Field").some(x => x.getName() == attribute.getName())) {
            continue;
        }
        let field = createElement("DTO-Field", attribute.getName(), dto.id);
        field.typeReference.setType(attribute.typeReference.typeId);
        field.typeReference.setIsNullable(attribute.typeReference.isNullable);
        field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        field.setMapping(attribute.id);
        dtoUpdated = true;
    }
    if (dtoUpdated) {
        dto.collapse();
    }
}
