/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
var _a, _b;
const privateSettersOnly = ((_b = (_a = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")) === null || _a === void 0 ? void 0 : _a.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")) === null || _b === void 0 ? void 0 : _b.value) == "true";
function addDiagram(context, whatToAdd, diagramFolder) {
    var _a;
    if (diagramFolder == null) {
        diagramFolder = context.targetFolder;
    }
    let entity = context.dialogOptions.selectedEntity;
    if (DomainHelper.isAggregateRoot(entity)) {
        const aggregateRootFolderName = getAggregateRootFolderName(entity);
        const diagramElement = (_a = diagramFolder.getChildren("Diagram").find(x => x.getName() == aggregateRootFolderName)) !== null && _a !== void 0 ? _a : createElement("Diagram", aggregateRootFolderName, diagramFolder.id);
        diagramElement.loadDiagram();
        const diagram = getCurrentDiagram();
        diagram.layoutVisuals(whatToAdd, null, true);
    }
}
async function notifyUserOfLimitations(entity, dialogOptions) {
    if ((privateSettersOnly && !hasConstructor(entity)) && dialogOptions.canCreate) {
        await dialogService.warn(`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    }
    else if (!entityHasPrimaryKey(entity) && (dialogOptions.canDelete || dialogOptions.canQueryById || dialogOptions.canUpdate)) {
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
function getOrCreateEntityFolder(folderOrPackage, entity) {
    var _a;
    if (folderOrPackage.specialization == "Folder") {
        return element;
    }
    const folderName = getAggregateRootFolderName(entity);
    const folder = (_a = element.getChildren().find(x => x.getName() == pluralize(folderName))) !== null && _a !== void 0 ? _a : createElement("Folder", pluralize(folderName), element.id);
    return folder;
}
function getAggregateRootFolderName(entity) {
    return pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName());
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
;
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
        if (autoAddPrimaryKey && !isCreateMode) {
            CrudHelper.addPrimaryKeys(dto, domainElement, true);
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
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !CrudHelper.legacyPartitionKey(x));
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
/// <reference path="common.ts" />
/// <reference path="../../common/crudHelper.ts" />
class CrudCreationContext {
    constructor(element, dialogOptions, targetFolder, primaryKeys) {
        this.element = element;
        this.dialogOptions = dialogOptions;
        this.targetFolder = targetFolder;
        this.primaryKeys = primaryKeys;
    }
    hasPrimaryKey() {
        return this.primaryKeys.length > 0;
    }
}
async function interactionWrapper(element, preselectedClass, implementation) {
    let dialogOptions = await presentCrudOptionsDialog(preselectedClass);
    if (dialogOptions == null || dialogOptions.selectedEntity == null)
        return;
    const targetFolder = getOrCreateEntityFolder(element, dialogOptions.selectedEntity);
    const primaryKeys = DomainHelper.getPrimaryKeys(dialogOptions.selectedEntity);
    let context = new CrudCreationContext(element, dialogOptions, targetFolder, primaryKeys);
    implementation(context);
    notifyUserOfLimitations(dialogOptions.selectedEntity, dialogOptions);
}
async function presentCrudOptionsDialog(preselectedClass) {
    let dialogResult = null;
    if (!preselectedClass) {
        dialogResult = await CrudHelper.openCrudCreationDialog({
            includeOwnedRelationships: true,
            allowAbstract: false
        });
        if (!dialogResult) {
            return null;
        }
    }
    else {
        dialogResult = {
            selectedEntity: preselectedClass,
            canCreate: true,
            canUpdate: true,
            canQueryById: true,
            canQueryAll: true,
            canDelete: true,
            canDomain: true,
            selectedDomainOperationIds: []
        };
    }
    return dialogResult;
}
/// <reference path="common.ts" />
/// <reference path="crud-dialog.ts" />
const CrudApi = {
    createCQRSService,
    createTraditionalService
};
async function createCQRSService(element, preselectedClass) {
    interactionWrapper(element, preselectedClass, cqrsImplementation);
}
async function createTraditionalService(element, preselectedClass) {
    interactionWrapper(element, preselectedClass, traditionalImplementation);
}
async function cqrsImplementation(context) {
    let dialogResult = context.dialogOptions;
    let entity = dialogResult.selectedEntity;
    let targetFolder = context.targetFolder;
    let primaryKeys = context.primaryKeys;
    let hasPrimaryKey = context.hasPrimaryKey();
    let resultDto = null;
    /*
    if (dialogResult.canQueryById || dialogResult.canQueryAll) {
        resultDto = cqrsCrud.createCqrsResultTypeDto(entity, targetFolder);
    }

    if ((!privateSettersOnly || hasConstructor(entity)) && dialogResult.canCreate) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCreateCommand(entity, targetFolder, primaryKeys));
    }

    if ((hasPrimaryKey && !privateSettersOnly) && dialogResult.canUpdate) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsUpdateCommand(entity, targetFolder));
    }

    if (hasPrimaryKey && dialogResult.canQueryById) {
        convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindByIdQuery(entity, targetFolder, resultDto));
    }

    if (dialogResult.canQueryAll) {
        convertToAdvancedMapping.convertQuery(cqrsCrud.createCqrsFindAllQuery(entity, targetFolder, resultDto));
    }

    if (hasPrimaryKey && dialogResult.canDelete) {
        convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsDeleteCommand(entity, targetFolder));
    }

    if (dialogResult.canDomain) {
        const operations = DomainHelper.getCommandOperations(entity);
        for (const operation of operations) {
            if (!dialogResult.selectedDomainOperationIds.some(x => x == operation.id)) {
                continue;
            }
            convertToAdvancedMapping.convertCommand(cqrsCrud.createCqrsCallOperationCommand(entity, operation, targetFolder));
        }
    }*/
    addDiagram(context, context.targetFolder);
}
async function traditionalImplementation(context) {
    const package = element.getPackage();
    let dialogResult = context.dialogOptions;
    let entity = dialogResult.selectedEntity;
    let targetFolder = context.targetFolder;
    let primaryKeys = context.primaryKeys;
    let hasPrimaryKey = context.hasPrimaryKey();
    const serviceName = `${toPascalCase(pluralize(DomainHelper.ownerIsAggregateRoot(entity) ? DomainHelper.getOwningAggregate(entity).getName() : entity.getName()))}Service`;
    const existingService = element.specialization == "Service" ? element : package.getChildren("Service").find(x => x.getName() == serviceName);
    const service = existingService !== null && existingService !== void 0 ? existingService : createElement("Service", serviceName, package.id);
    /*
    let resultDto: MacroApi.Context.IElementApi = null;
    if (dialogResult.canQueryById || dialogResult.canQueryAll) {
        resultDto = servicesCrud.createMappedResultDto(entity, targetFolder);
    }

    if ((!privateSettersOnly || hasConstructor(entity)) && dialogResult.canCreate) {
        servicesCrud.createStandardCreateOperation(service, entity, targetFolder);
    }

    if (primaryKeys.length > 0 && (!privateSettersOnly && dialogResult.canUpdate)) {
        servicesCrud.createStandardUpdateOperation(service, entity, targetFolder);
    }

    if (primaryKeys.length > 0 && dialogResult.canQueryById) {
        servicesCrud.createStandardFindByIdOperation(service, entity, resultDto);
    }

    if (dialogResult.canQueryAll) {
        servicesCrud.createStandardFindAllOperation(service, entity, resultDto);
    }

    if (primaryKeys.length > 0) {
        if (dialogResult.canDelete) {
            servicesCrud.createStandardDeleteOperation(service, entity);
        }

        if (dialogResult.canDomain) {
            const operations = DomainHelper.getCommandOperations(entity);
            for (const operation of operations) {
                if (!dialogResult.selectedDomainOperationIds.some(x => x == operation.id)) {
                    continue;
                }
                servicesCrud.createCallOperationCommand(service, operation, entity, targetFolder);
            }
        }
    }

    service.getChildren("Operation").forEach(operation => {
        convertToAdvancedMapping.convertOperation(operation, entity);
    })*/
    addDiagram(context, service);
}
