/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
var _a, _b;
const privateSettersOnly = ((_b = (_a = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")) === null || _a === void 0 ? void 0 : _a.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")) === null || _b === void 0 ? void 0 : _b.value) == "true";
async function notifyUserOfLimitations(entity, dialogOptions) {
    if ((privateSettersOnly && !hasConstructor(entity)) && dialogOptions.canCreate) {
        await dialogService.warn(`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    }
    else if (!entityHasPrimaryKey(entity) && (dialogOptions.canDelete || dialogOptions.canQueryById || dialogOptions.canUpdate || dialogOptions.selectedDomainOperationIds.length > 0)) {
        await dialogService.warn(`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to no Primary Key on entity '${entity.getName()}'.

To avoid this limitation in the future, model a Primary Key on the entity.`);
    }
}
function hasConstructor(entity) {
    return entity.getChildren("Class Constructor").length > 0;
}
function entityHasPrimaryKey(entity) {
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);
    return primaryKeys.length > 0;
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
/// <reference path="common.ts" />
/// <reference path="../../common/domainHelper.ts" />
class CrudCreationContext {
    constructor(element, dialogOptions, primaryKeys) {
        this.element = element;
        this.dialogOptions = dialogOptions;
        this.primaryKeys = primaryKeys;
    }
    hasPrimaryKey() {
        return this.primaryKeys.length > 0;
    }
}
async function presentCrudOptionsDialog(preselectedClass, defaultDiagramId) {
    let dialogResult = null;
    //if (!preselectedClass) {
    dialogResult = await openCrudCreationDialog(preselectedClass === null || preselectedClass === void 0 ? void 0 : preselectedClass.id, defaultDiagramId);
    if (!dialogResult) {
        return null;
    }
    // } else {
    //     dialogResult = {
    //         selectedEntity: preselectedClass,
    //         diagramId: defaultDiagramId,
    //         canCreate: true,
    //         canUpdate: true,
    //         canQueryById: true,
    //         canQueryAll: true,
    //         canDelete: true,
    //         canDomain: true,
    //         selectedDomainOperationIds: []
    //     };
    // }
    return dialogResult;
}
function ClassSelectionFilter(element) {
    let entity = element;
    if (!entity || entity.specialization != "Class") {
        return false;
    }
    if (DomainHelper.isAggregateRoot(element)) {
        return true;
    }
    let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
    for (let i = 0; i < results.length; i++) {
        if (results[i].getOtherEnd().typeReference.isCollection) {
            return true;
        }
    }
    return false;
}
async function openCrudCreationDialog(preselectedClassId, defaultDiagramId) {
    var _a, _b;
    let classes = lookupTypesOf("Class").filter(x => ClassSelectionFilter(x));
    if (classes.length == 0) {
        await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
        return null;
    }
    let diagrams = lookupTypesOf("Diagram", false);
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
                value: preselectedClassId,
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
            },
            {
                id: "diagramId",
                fieldType: "select",
                label: "Add to Diagram",
                selectOptions: [{
                        id: "create-new-diagram",
                        description: "<Create New Diagram>",
                        additionalInfo: null
                    }].concat(diagrams.map(x => {
                    return {
                        id: x.id,
                        description: x.getName(),
                        additionalInfo: getClassAdditionalInfo(x)
                    };
                })),
                value: defaultDiagramId !== null && defaultDiagramId !== void 0 ? defaultDiagramId : "create-new-diagram",
                isRequired: true
            },
        ]
    });
    let foundEntity = lookup(dialogResult.entityId);
    var result = {
        selectedEntity: foundEntity,
        diagramId: dialogResult.diagramId == "create-new-diagram" ? null : dialogResult.diagramId,
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
                    isRequired: false,
                    treeViewOptions: {
                        rootNode: {
                            specializationId: "Class",
                            id: "root",
                            label: "Loading...",
                            isExpanded: true,
                            children: [],
                        },
                        isMultiSelect: true,
                        submitFormTriggers: ["double-click", "enter"],
                        selectableTypes: [
                            {
                                specializationId: "Class",
                                autoExpand: true,
                                autoSelectChildren: false,
                                isSelectable: false
                            },
                            {
                                specializationId: "Operation",
                                isSelectable: true
                            }
                        ]
                    }
                }
            ],
            onInitialize: async (formApi) => {
                // This somehow loads the tree correctly as opposed to populating in the structure above directly.
                const tree = formApi.getField("tree");
                tree.treeViewOptions.rootId = foundEntity.id;
            }
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
class ElementManager {
    constructor(innerElement, settings) {
        var _a;
        this.innerElement = innerElement;
        this.settings = settings;
        this.mappedElement = (_a = innerElement.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement();
    }
    get id() { return this.innerElement.id; }
    ;
    setReturnType(typeId, isCollection, isNullable) {
        this.innerElement.typeReference.setType(typeId);
        if (isCollection != null) {
            this.innerElement.typeReference.setIsCollection(isCollection);
        }
        if (isNullable != null) {
            this.innerElement.typeReference.setIsNullable(isNullable);
        }
        return this;
    }
    addChild(name, type) {
        var _a;
        let existingField = this.innerElement.getChildren(this.settings.childSpecialization)
            .find(c => { var _a; return c.getName().toLowerCase() == ServicesHelper.formatName(name, (_a = this.settings.childType) !== null && _a !== void 0 ? _a : "property").toLowerCase(); });
        let field = existingField !== null && existingField !== void 0 ? existingField : createElement(this.settings.childSpecialization, ServicesHelper.formatName(name, (_a = this.settings.childType) !== null && _a !== void 0 ? _a : "property"), this.innerElement.id);
        if (type != null) {
            if (typeof (type) === "string") {
                field.typeReference.setType(type);
                field.typeReference.setIsCollection(false);
                field.typeReference.setIsNullable(false);
            }
            else {
                field.typeReference.setType(type.toModel());
            }
        }
        return field;
    }
    addChildrenFrom(elements, options) {
        let order = 0;
        elements.forEach(e => {
            if (e.mapPath != null) {
                if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => { var _a, _b; return ((_b = (_a = x.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.id) == e.id; })) {
                    return;
                }
            }
            else if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
            let field = this.addChild(e.name, e.typeId);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
            if (options === null || options === void 0 ? void 0 : options.addToTop) {
                field.setOrder(order++);
            }
            if (this.mappedElement != null && e.mapPath) {
                field.setMapping(e.mapPath);
            }
        });
        return this;
    }
    mapToElement(param1, mappingSettingsId) {
        let elementIds;
        let element;
        if (Array.isArray(param1)) {
            elementIds = param1;
            element = lookup(elementIds[elementIds.length - 1]);
        }
        else {
            elementIds = [param1.id];
            element = param1;
        }
        this.mappedElement = element;
        this.innerElement.setMapping(elementIds, mappingSettingsId);
        return this;
    }
    getElement() {
        return this.innerElement;
    }
    collapse() {
        this.innerElement.collapse();
    }
}
/// <reference path="common.ts" />
var MappingType;
(function (MappingType) {
    MappingType[MappingType["Navigation"] = 1] = "Navigation";
    MappingType[MappingType["TypeMap"] = 2] = "TypeMap";
})(MappingType || (MappingType = {}));
class EntityProjector {
    addMapping(type, sourceRelative, targetRelative, changeCurrent = false) {
        let result = { targetPops: 0, sourcePops: 0 };
        this.mappings.push({
            type: type,
            sourcePath: this.sourcePath.concat(sourceRelative),
            targetPath: this.targetPath.concat(targetRelative),
            targetPropertyStart: targetRelative[0]
        });
        if (type == MappingType.Navigation && changeCurrent) {
            this.sourcePath.push(...sourceRelative);
            this.targetPath.push(...targetRelative);
            result.sourcePops = sourceRelative.length;
            result.targetPops = targetRelative.length;
        }
        return result;
    }
    popMapping(pops) {
        for (let i = 0; i < pops.sourcePops; i++) {
            this.sourcePath.pop();
        }
        for (let i = 0; i < pops.targetPops; i++) {
            this.targetPath.pop();
        }
    }
    constructor() {
        this.mappings = [];
        this.sourcePath = []; //Dto
        this.targetPath = []; //Entity
        this.isChild = false;
        this.addMandatoryRelationships = false;
        this.visited = new Set();
    }
    getMappings() {
        return this.mappings;
    }
    getTarget() {
        return this.target;
    }
    getEntityConstructor(entity) {
        return entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
    }
    createCreateCommand(commandName, entity, folder) {
        this.addMandatoryRelationships = true;
        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        const entityCtor = this.getEntityConstructor(entity);
        if (entityCtor != null) {
            entity = entityCtor;
            this.targetPath.push(entityCtor.id);
        }
        //Not this is the entity or the ctor depending
        this.target = entity;
        let result = this.populateContract(dto, entity, true, folder);
        return result;
    }
    createOrGetCallOperation(operation, entity, folder) {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);
        const commandName = `${operationName}${entity.getName()}Command`;
        const existing = folder.getChildren().find(x => x.getName() == commandName);
        if (existing) {
            return existing;
        }
        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        this.targetPath.push(operation.id);
        //Not this is the entity or the ctor depending
        this.target = operation;
        let result = this.populateContract(dto, operation, false, folder);
        return result;
    }
    createUpdateCommand(commandName, entity, folder) {
        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.addMandatoryRelationships = true;
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        this.target = entity;
        let result = this.populateContract(dto, entity, false, folder);
        return result;
    }
    createDeleteCommand(commandName, entity, folder) {
        let dto = this.getOrCreateElement(commandName, "Command", folder);
        let attributes = this.getAttributesWithMapPath(entity, (x) => x.hasStereotype("Primary Key"));
        this.addDtoFieldsInternal(attributes, false, entity, dto, folder, true);
        dto.collapse();
        return dto;
    }
    createFindByIdQuery(expectedQueryName, entity, folder, resultDto) {
        let query = createElement("Query", expectedQueryName, folder.id);
        query.typeReference.setType(resultDto.id);
        let attributes = this.getAttributesWithMapPath(entity, (x) => x.hasStereotype("Primary Key"));
        this.addDtoFieldsInternal(attributes, false, entity, query, folder, true);
        query.collapse();
        return query;
    }
    createFindAllQuery(expectedQueryName, entity, folder, resultDto) {
        let query = createElement("Query", expectedQueryName, folder.id);
        query.typeReference.setType(resultDto.id);
        query.typeReference.setIsCollection(true);
        query.collapse();
        return query;
    }
    createOrGetOperationDto(operationManager, entity, folder, createMode, inbound = false, addMandatoryRelationships = false) {
        let operation = operationManager.getElement();
        let dtoName = `${operation.getName()}Dto`;
        let existing = folder.getChildren().find(x => x.getName() == dtoName);
        if (existing) {
            return existing;
        }
        this.addMandatoryRelationships = addMandatoryRelationships;
        let dto = this.getOrCreateElement(dtoName, "DTO", folder);
        let dtoParam = operationManager.addChild("dto", dto.id);
        this.sourcePath.push(operation.id);
        this.sourcePath.push(dtoParam.id);
        this.targetPath.push(entity.id);
        if (inbound) {
            const entityCtor = this.getEntityConstructor(entity);
            if (entityCtor != null) {
                entity = entityCtor;
                this.targetPath.push(entityCtor.id);
            }
        }
        //Not this is the entity or the ctor depending
        this.target = entity;
        let result = this.populateContract(dto, entity, createMode, folder);
        return result;
    }
    createOrGetDto(entity, folder, inbound = false) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}Dto`;
        let existing = folder.getChildren().find(x => x.getName() == dtoName);
        if (existing) {
            return existing;
        }
        let dto = this.getOrCreateElement(dtoName, "DTO", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        if (inbound) {
            const entityCtor = this.getEntityConstructor(entity);
            if (entityCtor != null) {
                entity = entityCtor;
                this.targetPath.push(entityCtor.id);
            }
        }
        //Not this is the entity or the ctor depending
        this.target = entity;
        let result = this.populateContract(dto, entity, false, folder);
        return result;
    }
    populateContract(contract, entity, createMode, folder) {
        if (entity.specialization == "Class Constructor" || entity.specialization == "Operation") {
            this.addMapping(MappingType.TypeMap, [contract.id], [entity.id]);
            this.addDtoFieldsForCtor(createMode, entity, contract, folder);
        }
        else {
            this.addMapping(MappingType.TypeMap, [contract.id], [entity.id]);
            this.addDtoFields(createMode, entity, contract, folder, false);
        }
        return contract;
    }
    getOrCreateContract(elementName, elementType, entity, createMode, folder, inbound = false, mandatoryRelationship = false) {
        let dto = this.getOrCreateElement(elementName, elementType, folder);
        const entityCtor = entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (inbound && entityCtor != null) {
            this.addMapping(MappingType.TypeMap, [dto.id], [entity.id, entityCtor.id]);
            this.addDtoFieldsForCtor(createMode, entityCtor, dto, folder);
        }
        else {
            this.addMapping(MappingType.TypeMap, [dto.id], [entity.id]);
            this.addDtoFields(createMode, entity, dto, folder, inbound, mandatoryRelationship);
        }
        return dto;
    }
    addDtoFields(createMode, entity, dto, folder, inbound = false, mandatoryRelationship = false) {
        let dtoUpdated = false;
        let domainElement = entity;
        let attributesWithMapPaths = createMode ?
            this.getAttributesWithMapPath(domainElement, (x) => this.standardAttributeFilter(x) && !this.generatedPKFilter(x)) :
            this.getAttributesWithMapPath(domainElement, (x) => this.standardAttributeFilter(x) && (!mandatoryRelationship || !this.compositePKFilter(x)));
        this.addDtoFieldsInternal(attributesWithMapPaths, createMode, entity, dto, folder, inbound);
    }
    addDtoFieldsInternal(attributes, createMode, entity, dto, folder, inbound = false) {
        let dtoUpdated = false;
        let domainElement = entity;
        for (let keyName of Object.keys(attributes)) {
            let entry = attributes[keyName];
            if (createMode && this.isChild == true && CrudHelper.isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            //console.warn("Field : " + entry.name + " , mappath =" + entry.mapPath);
            let pops = this.addMapping(MappingType.Navigation, [field.id], entry.mapPath, DomainHelper.isComplexTypeById(entry.typeId));
            if (DomainHelper.isComplexTypeById(entry.typeId)) {
                let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                let entityField = lookup(entry.id);
                let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, inbound);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(entry.typeId);
            }
            this.popMapping(pops);
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            dtoUpdated = true;
        }
        if (this.addMandatoryRelationships) {
            if (!this.visited.has(entity.id)) {
                this.visited.add(entity.id);
                this.isChild = true;
                let requiredAssociations = DomainHelper.getMandatoryAssociationsWithMapPath(entity);
                for (let entry of requiredAssociations) {
                    let entityField = lookup(entry.id);
                    if (!this.visited.has(entityField.typeReference.getType().id)) {
                        let field = createElement("DTO-Field", entry.name, dto.id);
                        let pops = this.addMapping(MappingType.Navigation, [field.id], entry.mapPath, true);
                        let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                        let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, inbound, true);
                        field.typeReference.setType(newDto.id);
                        this.popMapping(pops);
                        field.typeReference.setIsNullable(entry.isNullable);
                        field.typeReference.setIsCollection(entry.isCollection);
                        dtoUpdated = true;
                    }
                }
            }
        }
        if (dtoUpdated) {
            dto.collapse();
        }
    }
    addDtoFieldsForCtor(createMode, ctor, dto, folder) {
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
            let pops = this.addMapping(MappingType.Navigation, [field.id], e.mapPath, DomainHelper.isComplexTypeById(e.typeId));
            if (DomainHelper.isComplexTypeById(e.typeId)) {
                let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                let entityField = lookup(e.id);
                let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, false);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(e.typeId);
            }
            this.popMapping(pops);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
        dto.collapse();
    }
    getBaseNameForElement(owningAggregate, entity, entityIsMany) {
        // Keeping 'owningAggregate' in case we still need to use it as part of the name one day
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }
    getOrCreateElement(elementName, elementType, parentElement) {
        let existingDto = parentElement.getChildren(elementType).filter(x => x.getName() === elementName)[0];
        if (existingDto) {
            return existingDto;
        }
        let dto = createElement(elementType, elementName, parentElement.id);
        return dto;
    }
    standardAttributeFilter(x) {
        var _a;
        return !CrudHelper.legacyPartitionKey(x) &&
            (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || ((_a = x.getMetadata("set-by-infrastructure")) === null || _a === void 0 ? void 0 : _a.toLocaleLowerCase()) !== "true"));
    }
    generatedPKFilter(x) {
        return x.hasStereotype("Primary Key") && (!x.getStereotype("Primary Key").hasProperty("Data source") || x.getStereotype("Primary Key").getProperty("Data source").value != "User supplied");
    }
    compositePKFilter(x) {
        let pk = x.hasStereotype("Primary Key");
        let entity = x.getParent();
        return pk && DomainHelper.getOwningAggregateRecursive(entity) != null;
    }
    getAttributesWithMapPath(entity, attributeFilter) {
        if (attributeFilter == null) {
            attributeFilter = (x) => this.standardAttributeFilter(x) && !x.hasStereotype("Primary Key");
        }
        let attrDict = Object.create(null);
        let attributes = entity.getChildren("Attribute")
            .filter(attributeFilter);
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
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
            let baseKeys = nextEntity.getChildren("Attribute")
                .filter(attributeFilter);
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
}
/// <reference path="common.ts" />
/// <reference path="crud-dialog.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../common/elementManager.ts" />
/// <reference path="project-from-entity.ts" />
class CrudStrategy {
    constructor() {
        this.resultDto = null;
        //protected owningAggregate?: IElementApi = null;
        this.entity = null;
        this.context = null;
        this.targetFolder = null;
        this.primaryKeys = null;
    }
    async askUser(element, preselectedClass, diagram) {
        if (diagram == null) {
            for (const e of [element].concat(element.getParents().reverse())) {
                const diagramTypeId = "8c90aca5-86f4-47f1-bd58-116fe79f5c55"; // only supported in 4.5.8 and later
                if (e.getChildren("Diagram").length > 0) {
                    diagram = e.getChildren("Diagram")[0];
                    break;
                }
            }
        }
        let dialogOptions = await presentCrudOptionsDialog(preselectedClass, diagram === null || diagram === void 0 ? void 0 : diagram.id);
        if (dialogOptions == null || dialogOptions.selectedEntity == null)
            return null;
        const primaryKeys = DomainHelper.getPrimaryKeys(dialogOptions.selectedEntity);
        let context = new CrudCreationContext(element, dialogOptions, primaryKeys);
        return context;
    }
    static getOrCreateEntityFolder(folderOrPackage, entity) {
        var _a;
        if (folderOrPackage.specialization == "Folder") {
            return folderOrPackage;
        }
        const folderName = this.getAggregateRootFolderName(entity);
        const folder = (_a = folderOrPackage.getChildren().find(x => x.getName() == pluralize(folderName))) !== null && _a !== void 0 ? _a : createElement("Folder", pluralize(folderName), folderOrPackage.id);
        return folder;
    }
    static getAggregateRootFolderName(entity) {
        const owningAggregate = DomainHelper.getOwningAggregateRecursive(entity);
        return pluralize(owningAggregate != null ? owningAggregate.getName() : entity.getName());
    }
    async execute(element, preselectedClass, diagramElement) {
        var _a;
        this.context = await this.askUser(element, preselectedClass, diagramElement);
        if (this.context == null)
            return;
        await this.executeWithContext(this.context, diagramElement);
        const targetPoint = diagramElement != null && diagramElement.id == this.context.dialogOptions.diagramId ? getCurrentDiagram().mousePosition : null;
        diagramElement = (_a = lookup(this.context.dialogOptions.diagramId)) !== null && _a !== void 0 ? _a : this.getOrCreateDiagram(this.targetFolder);
        diagramElement.loadDiagram();
        const diagram = getCurrentDiagram();
        this.doAddElementsToDiagram(diagram, targetPoint);
        await notifyUserOfLimitations(this.context.dialogOptions.selectedEntity, this.context.dialogOptions);
    }
    async executeForOperation(domainOperationElement, diagramElement) {
        var _a;
        if (domainOperationElement.specialization != "Operation") {
            throw new Error("Element is not an operation");
        }
        if (((_a = domainOperationElement.getParent()) === null || _a === void 0 ? void 0 : _a.specialization) !== "Class") {
            throw new Error("Operation's parent is not a class");
        }
        const operation = domainOperationElement;
        const entity = operation.getParent();
        const targetFolder = CrudStrategy.getOrCreateEntityFolder(diagramElement.getOwner().getPackage(), entity);
        const context = {
            element: targetFolder,
            dialogOptions: {
                selectedEntity: entity,
                canCreate: false,
                canUpdate: false,
                canDelete: false,
                canQueryById: false,
                canQueryAll: false,
                canDomain: true,
                selectedDomainOperationIds: [operation.id],
                diagramId: diagramElement.id
            },
            primaryKeys: DomainHelper.getPrimaryKeys(entity),
            hasPrimaryKey: () => DomainHelper.getPrimaryKeys(entity).length > 0
        };
        const createdElements = await this.executeWithContext(context, diagramElement.getOwner());
        if (createdElements.length > 0) {
            this.doAddElementToDiagram(createdElements[createdElements.length - 1], diagramElement);
        }
    }
    async executeWithContext(context, diagramElement) {
        var _a;
        this.context = context;
        const createdElements = [];
        let dialogOptions = this.context.dialogOptions;
        this.entity = dialogOptions.selectedEntity;
        this.primaryKeys = this.context.primaryKeys;
        let hasPrimaryKey = this.context.hasPrimaryKey();
        if (!await this.validate()) {
            return createdElements;
        }
        const owningAggregate = DomainHelper.getOwningAggregateRecursive(this.entity);
        this.targetFolder = CrudStrategy.getOrCreateEntityFolder(this.context.element, this.entity);
        this.initialize(this.context);
        if (dialogOptions.canQueryById || dialogOptions.canQueryAll) {
            let projector = new EntityProjector();
            this.resultDto = projector.createOrGetDto(this.entity, this.targetFolder);
            if (projector.getMappings().length > 0) {
                this.addBasicMapping(projector.getMappings());
            }
            this.resultDto.collapse();
        }
        if ((!privateSettersOnly || hasConstructor(this.entity)) && dialogOptions.canCreate) {
            let x = this.doCreate();
            if (owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
            createdElements.push(x);
        }
        if ((hasPrimaryKey && !privateSettersOnly) && dialogOptions.canUpdate) {
            let x = this.doUpdate();
            if (owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
            createdElements.push(x);
        }
        if (hasPrimaryKey && dialogOptions.canQueryById) {
            let x = this.doGetById();
            if (owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
            createdElements.push(x);
        }
        if (dialogOptions.canQueryAll) {
            let x = this.doGetAll();
            if (owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
            createdElements.push(x);
        }
        if (hasPrimaryKey && dialogOptions.canDelete) {
            let x = this.doDelete();
            if (owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
            createdElements.push(x);
        }
        if (dialogOptions.canDomain) {
            const operations = DomainHelper.getCommandOperations(this.entity);
            for (const operation of operations) {
                if (!this.context.dialogOptions.selectedDomainOperationIds.some(x => x == operation.id)) {
                    continue;
                }
                let operationResultDto = null;
                if (operation.typeReference != null) {
                    if (DomainHelper.isComplexType((_a = operation.typeReference) === null || _a === void 0 ? void 0 : _a.getType())) {
                        let projector2 = new EntityProjector();
                        let from = lookup(operation.typeReference.getTypeId());
                        operationResultDto = projector2.createOrGetDto(from, this.targetFolder);
                        if (projector2.getMappings().length > 0) {
                            this.addBasicMapping(projector2.getMappings());
                        }
                    }
                }
                let x = this.doOperation(operation, operationResultDto);
                if (owningAggregate != null) {
                    this.AddAggregateKeys(x);
                }
                x.collapse();
                createdElements.push(x);
            }
        }
        return createdElements;
    }
    async validate() {
        if (DomainHelper.getOwnersRecursive(this.entity).length > 1) {
            const owners = DomainHelper.getOwnersRecursive(this.entity).map(item => item.getName()).join(", ");
            await dialogService.warn(`Entity has multiple owners.
The entity '${this.entity.getName()}' has multiple Aggregates owning it [${owners}].

Compositional Entities (black diamond) must have 1 owner. Please adjust the associations accordingly.`);
            return false;
        }
        return true;
    }
    doAdvancedMappingCreate(projector, source) {
        if (projector.getMappings().length > 0) {
            let target = projector.getTarget();
            let action = createAssociation("Create Entity Action", source.id, target.id);
            let mapping = action.createAdvancedMapping(source.id, this.entity.id);
            mapping.addMappedEnd("Invocation Mapping", [source.id], [target.id]);
            this.addAdvancedMappingEnds("Data Mapping", source, mapping, projector.getMappings());
        }
    }
    doAdvancedMappingDelete(mappings, source) {
        if (mappings.length > 0) {
            let action = createAssociation("Delete Entity Action", source.id, this.entity.id);
            let mapping = action.createAdvancedMapping(source.id, this.entity.id);
            this.addAdvancedMappingEnds("Filter Mapping", source, mapping, mappings);
        }
    }
    doAdvancedMappingGetById(mappings, source) {
        if (mappings.length > 0) {
            let action = createAssociation("Query Entity Action", source.id, this.entity.id);
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, mappings);
        }
    }
    doAdvancedMappingUpdate(projector, source) {
        if (projector.getMappings().length > 0) {
            let action = createAssociation("Update Entity Action", source.id, this.entity.id);
            //remove PKs from Update
            let updateMappingEnds = projector.getMappings().filter(x => {
                const last = x.targetPath[x.targetPath.length - 1];
                return !this.primaryKeys.some(pk => pk.id == last);
            });
            let queryMappingEnds = this.createQueryMappingEnds(source);
            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, queryMappingEnds);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(source.id, this.entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            this.addAdvancedMappingEnds("Data Mapping", source, updateMapping, updateMappingEnds);
        }
    }
    doAdvancedMappingGetAll(source) {
        let action = createAssociation("Query Entity Action", source.id, this.entity.id);
        action.typeReference.setIsCollection(true);
        action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
    }
    doAdvancedMappingCallOperation(projector, source) {
        if (projector.getMappings().length > 0) {
            let target = projector.getTarget();
            let action = createAssociation("Update Entity Action", source.id, target.id);
            //remove PKs from Update
            let updateMappingEnds = projector.getMappings().filter(x => {
                const last = x.targetPath[x.targetPath.length - 1];
                return !this.primaryKeys.some(pk => pk.id == last);
            });
            let queryMappingEnds = this.createQueryMappingEnds(source);
            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, queryMappingEnds);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(source.id, this.entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            updateMapping.addMappedEnd("Invocation Mapping", [source.id], [target.id]);
            this.addAdvancedMappingEnds("Data Mapping", source, updateMapping, updateMappingEnds);
        }
    }
    createQueryMappingEnds(source) {
        let queryMappingEnds = [];
        for (const pk of Object.values(this.primaryKeys)) {
            var dtoField = source.getChildren().find(x => x.getName() == pk.name);
            queryMappingEnds.push({ type: MappingType.Navigation, sourcePath: [dtoField.id], targetPath: pk.mapPath, targetPropertyStart: pk.mapPath[0] });
        }
        return queryMappingEnds;
    }
    getOrCreateDiagram(diagramFolder) {
        var _a;
        if (diagramFolder == null) {
            diagramFolder = this.targetFolder;
        }
        let entity = this.entity;
        const aggregateRootFolderName = CrudStrategy.getAggregateRootFolderName(entity);
        const diagramElement = (_a = diagramFolder.getChildren("Diagram").find(x => x.getName() == aggregateRootFolderName)) !== null && _a !== void 0 ? _a : createElement("Diagram", aggregateRootFolderName, diagramFolder.id);
        return diagramElement;
    }
    addBasicMapping(mappings) {
        mappings.forEach(m => {
            var _a;
            let dtoPart = lookup(m.sourcePath.slice(-1)[0]);
            //Work around for SetMapping clearing type in some scenarios.
            let previousType = (_a = dtoPart.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId();
            //Some property paths are multiple entries like "base.id"
            if (m.type == MappingType.Navigation) {
                //console.warn(m.type + ":" + m.sourcePath.map(x => lookup(x).getName()).join('.') + "->" + m.targetPath.map(x => lookup(x).getName()).join('.'));
                const index = m.targetPath.indexOf(m.targetPropertyStart);
                if (index === -1)
                    return; // value not found
                dtoPart.setMapping(m.targetPath.slice(index));
            }
            else {
                dtoPart.setMapping(m.targetPath.slice(-1)[0]);
            }
            if (previousType != null) {
                dtoPart.typeReference.setType(previousType);
            }
        });
    }
    addAdvancedMappingEnds(mappingType, element, mapping, mappings) {
        mappings.forEach(m => {
            var _a, _b, _c;
            if (m.type == MappingType.Navigation) {
                //console.warn(m.type + ":" + m.sourcePath.map(x => lookup(x).getName()).join('.') + "->" + m.targetPath.map(x => lookup(x).getName()).join('.'));
                let dtoPart = lookup(m.sourcePath.slice(-1)[0]);
                let mappedElementId = m.targetPath.slice(-1)[0];
                let element = lookup(mappedElementId);
                if (element.specialization == "Class Constructor") {
                    mapping.addMappedEnd("Invocation Mapping", [element.id], m.targetPath);
                }
                else {
                    if ((((_b = (_a = dtoPart.typeReference) === null || _a === void 0 ? void 0 : _a.getType()) === null || _b === void 0 ? void 0 : _b.specialization) != "DTO" || ((_c = dtoPart.typeReference) === null || _c === void 0 ? void 0 : _c.getIsCollection()))) {
                        mapping.addMappedEnd(mappingType, m.sourcePath, m.targetPath);
                    }
                }
            }
        });
    }
    AddAggregateKeys(element) {
        var _a;
        //Have to do the reverse so setOrder works
        let keys = (_a = DomainHelper.getOwningAggregateKeyChain(this.entity)) === null || _a === void 0 ? void 0 : _a.reverse();
        keys === null || keys === void 0 ? void 0 : keys.forEach((pk) => {
            if (!element.getChildren().some(x => x.getName().toLowerCase() == pk.expectedName.toLowerCase())) {
                let field = this.addMissingAggregateKey(element, pk.expectedName);
                field.typeReference.setType(pk.attribute.typeReference.getTypeId());
                field.setOrder(0);
            }
            else {
                let field = element.getChildren().find(x => x.getName().toLowerCase() == pk.expectedName.toLowerCase());
                field.setOrder(0);
            }
        });
    }
}
/// <reference path="strategy-base.ts" />
class CQRSCrudStrategy extends CrudStrategy {
    initialize(context) {
    }
    doCreate() {
        const commandName = `Create${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing)
            return existing;
        let projector = new EntityProjector();
        let command = projector.createCreateCommand(commandName, this.entity, this.targetFolder);
        const surrogateKey = this.primaryKeys.length === 1;
        if (surrogateKey) {
            command.typeReference.setType(this.primaryKeys[0].typeId);
        }
        this.doAdvancedMappingCreate(projector, command);
        return command;
    }
    doUpdate() {
        const commandName = `Update${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing)
            return existing;
        let projector = new EntityProjector();
        let command = projector.createUpdateCommand(commandName, this.entity, this.targetFolder);
        this.doAdvancedMappingUpdate(projector, command);
        return command;
    }
    doDelete() {
        const commandName = `Delete${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing)
            return existing;
        let projector = new EntityProjector();
        let command = projector.createDeleteCommand(commandName, this.entity, this.targetFolder);
        this.doAdvancedMappingDelete(projector.getMappings(), command);
        return command;
    }
    doGetById() {
        const queryName = `Get${this.getElementBaseName(this.entity, false)}ByIdQuery`;
        const existing = this.elementExists(queryName);
        if (existing)
            return existing;
        let projector = new EntityProjector();
        let query = projector.createFindByIdQuery(queryName, this.entity, this.targetFolder, this.resultDto);
        this.doAdvancedMappingGetById(projector.getMappings(), query);
        return query;
    }
    doGetAll() {
        const queryName = `Get${this.getElementBaseName(this.entity, true)}Query`;
        const existing = this.elementExists(queryName);
        if (existing)
            return existing;
        let projector = new EntityProjector();
        let query = projector.createFindAllQuery(queryName, this.entity, this.targetFolder, this.resultDto);
        this.doAdvancedMappingGetAll(query);
        return query;
    }
    doOperation(operation, operationResultDto) {
        let projector = new EntityProjector();
        let command = projector.createOrGetCallOperation(operation, this.entity, this.targetFolder);
        if (operationResultDto) {
            command.typeReference.setType(operationResultDto.id);
        }
        // Add Aggregate PK to command for Query
        let keys = this.primaryKeys.reverse();
        keys.forEach((pk) => {
            if (!command.getChildren().some(x => x.getName().toLowerCase() == pk.name.toLowerCase())) {
                let field = createElement("DTO-Field", pk.name, command.id);
                field.typeReference.setType(pk.typeId);
                field.setOrder(0);
            }
            else {
                let field = command.getChildren().find(x => x.getName().toLowerCase() == pk.name.toLowerCase());
                field.setOrder(0);
            }
        });
        this.doAdvancedMappingCallOperation(projector, command);
        return command;
    }
    getElementBaseName(entity, entityIsMany) {
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }
    elementExists(elementName) {
        return this.targetFolder.getChildren().find(x => x.getName() == elementName);
    }
    doAddElementsToDiagram(diagram, addAtPoint) {
        const space = diagram.findEmptySpace(addAtPoint !== null && addAtPoint !== void 0 ? addAtPoint : diagram.getViewPort().getCenter(), { width: 500, height: 550 });
        const visuals = diagram.layoutVisuals(this.targetFolder, space, true);
        diagram.selectVisualsForElements(visuals.map(x => x.id));
    }
    doAddElementToDiagram(element, diagram) {
        const existingElements = this.entity.getAssociations()
            .concat(this.entity.getChildren("Operation").map(x => x.getAssociations()).flat())
            .map(x => { var _a; return (_a = x.typeReference) === null || _a === void 0 ? void 0 : _a.getType(); })
            .filter(x => x != null && diagram.getVisual(x) != null && (x.specialization === "Command" || x.specialization === "Query"));
        if (existingElements.length > 0) {
            const lowestPlacedElement = existingElements.reduce((lowest, current) => {
                const lowestVisual = diagram.getVisual(lowest.id);
                const currentVisual = diagram.getVisual(current.id);
                return (currentVisual.getPosition().y > lowestVisual.getPosition().y) ? current : lowest;
            });
            const lastVisual = diagram.getVisual(lowestPlacedElement.id);
            let space = lastVisual.getPosition();
            space.y += lastVisual.getSize().height + 100;
            space.x += 100; // Not sure why but its X auto-moved to the left. Let's move it to the right.
            diagram.layoutVisuals([element.id], space);
        }
    }
    addMissingAggregateKey(element, name) {
        return createElement("DTO-Field", name, element.id);
    }
}
/// <reference path="strategy-base.ts" />
class TraditionalServicesStrategy extends CrudStrategy {
    initialize(context) {
        const intentPackage = context.element.getPackage();
        let entity = context.dialogOptions.selectedEntity;
        const owningAggregate = DomainHelper.getOwningAggregateRecursive(this.entity);
        const serviceName = `${toPascalCase(pluralize(owningAggregate != null ? owningAggregate.getName() : entity.getName()))}Service`;
        const existingService = context.element.specialization == "Service" ? context.element : intentPackage.getChildren("Service").find(x => x.getName() == serviceName);
        this.service = existingService !== null && existingService !== void 0 ? existingService : createElement("Service", serviceName, intentPackage.id);
    }
    doCreate() {
        let operationName = `Create${this.entity.getName()}`;
        const existing = this.operationExists(operationName);
        if (existing) {
            existing.typeReference.setType(this.primaryKeys[0].typeId);
            return existing;
        }
        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });
        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, this.entity, this.targetFolder, true, true, true);
        if (this.primaryKeys.length == 1) {
            operationManager.setReturnType(this.primaryKeys[0].typeId);
        }
        this.doAdvancedMappingCreate(projector, operationManager.getElement());
        return operationManager.getElement();
    }
    doUpdate() {
        let operationName = `Update${this.entity.getName()}`;
        const existing = this.operationExists(operationName);
        if (existing)
            return existing;
        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });
        let operation = operationManager.getElement();
        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
        }
        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, this.entity, this.targetFolder, true, true, true);
        this.doAdvancedMappingUpdate(projector, operationManager.getElement());
        return operationManager.getElement();
    }
    doDelete() {
        let operationName = `Delete${this.entity.getName()}`;
        const existing = this.operationExists(operationName);
        if (existing)
            return existing;
        let operation = createElement("Operation", operationName, this.service.id);
        let mappings = this.addPkParameters(operation);
        this.doAdvancedMappingDelete(mappings, operation);
        return operation;
    }
    doGetById() {
        let operationName = `Find${this.entity.getName()}ById`;
        const existing = this.operationExists(operationName);
        if (existing)
            return existing;
        let operation = createElement("Operation", operationName, this.service.id);
        let mappings = this.addPkParameters(operation);
        operation.typeReference.setType(this.resultDto.id);
        this.doAdvancedMappingGetById(mappings, operation);
        return operation;
    }
    doGetAll() {
        let operationName = `Find${pluralize(this.entity.getName())}`;
        const existing = this.operationExists(operationName);
        if (existing)
            return existing;
        let operation = createElement("Operation", operationName, this.service.id);
        operation.typeReference.setType(this.resultDto.id);
        operation.typeReference.setIsCollection(true);
        this.doAdvancedMappingGetAll(operation);
        return operation;
    }
    doOperation(domainOperation, operationResultDto) {
        let operationName = domainOperation.getName();
        const existing = this.operationExists(operationName);
        if (existing)
            return existing;
        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });
        let operation = operationManager.getElement();
        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
        }
        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, domainOperation, this.targetFolder, false, true);
        if (operationResultDto) {
            operationManager.setReturnType(operationResultDto.id);
        }
        this.doAdvancedMappingCallOperation(projector, operationManager.getElement());
        return operation;
    }
    addPkParameters(operation) {
        let mappings = [];
        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
            mappings.push({ type: MappingType.Navigation, sourcePath: [param.id], targetPath: pk.mapPath, targetPropertyStart: pk.mapPath[0] });
        }
        return mappings;
    }
    operationExists(operationName) {
        return this.service.getChildren().find(x => x.getName() === operationName);
    }
    doAddElementsToDiagram(diagram, addAtPoint) {
        const space = diagram.findEmptySpace(addAtPoint !== null && addAtPoint !== void 0 ? addAtPoint : diagram.getViewPort().getCenter(), { width: 500, height: 200 });
        const visuals = diagram.layoutVisuals(this.service, space, true);
        diagram.selectVisualsForElements(visuals.map(x => x.id));
    }
    doAddElementToDiagram(element, diagram) {
        this.doAddElementsToDiagram(diagram, diagram.mousePosition);
    }
    addMissingAggregateKey(element, name) {
        return createElement("Parameter", toCamelCase(name), element.id);
    }
}
/// <reference path="strategy-cqrs.ts" />
/// <reference path="strategy-traditional.ts" />
let CrudApi = {
    createCQRSService,
    createTraditionalService,
    createCQRSServiceForOperation,
    createTraditionalServiceForOperation
};
async function createCQRSService(element, preselectedClass, diagramElement) {
    let strategy = new CQRSCrudStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
}
async function createTraditionalService(element, preselectedClass, diagramElement) {
    let strategy = new TraditionalServicesStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
}
async function createCQRSServiceForOperation(domainOperationElement, diagramElement) {
    let strategy = new CQRSCrudStrategy();
    await strategy.executeForOperation(domainOperationElement, diagramElement);
}
async function createTraditionalServiceForOperation(domainOperationElement, diagramElement) {
    let strategy = new TraditionalServicesStrategy();
    await strategy.executeForOperation(domainOperationElement, diagramElement);
}
