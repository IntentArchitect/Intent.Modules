/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
const diagramSpecializationId = "8c90aca5-86f4-47f1-bd58-116fe79f5c55";
async function getOrCreateDiagram(folder, repositoryOperation, title) {
    let diagrams = lookupTypesOf("Diagram", false);
    const newDiagramOption = "create-new-diagram";
    const noDiagramOption = "no-diagram";
    const repositoryId = repositoryOperation.getParent();
    let dialogResult = await dialogService.openForm({
        title: title,
        fields: [
            {
                id: "diagramId",
                fieldType: "select",
                label: "Add to Diagram",
                selectOptions: [{
                        id: noDiagramOption,
                        description: "<None>",
                    },
                    {
                        id: newDiagramOption,
                        description: "<Create New Diagram>",
                    }].concat(diagrams.map(x => {
                    return {
                        id: x.id,
                        description: x.getName(),
                    };
                })),
                value: getCurrentDiagram()?.getOwner()?.id ?? noDiagramOption,
                isRequired: true
            },
        ]
    });
    if (dialogResult.diagramId == noDiagramOption) {
        return null;
    }
    let diagramElement = dialogResult.diagramId == newDiagramOption
        ? createElement(diagramSpecializationId, folder.getName(), folder.id)
        : lookup(dialogResult.diagramId);
    await diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    if (!diagram.getVisual(repositoryId)) {
        diagram.layoutVisuals(repositoryId);
    }
    return diagram;
}
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
function getPackageSelectItemOptions(packages, packageTypeName) {
    return {
        items: packages,
        getId(item) {
            return item.id;
        },
        getDisplayName(item) {
            return item.getName();
        },
        getItemNotFoundMessage(itemId) {
            return `No ${packageTypeName} found with id "${itemId}".`;
        },
        getNoItemsFoundMessage() {
            return `No packages of type ${packageTypeName} could be found.`;
        },
    };
}
function getElementSelectItemOptions(elements, elementTypeName, relevantPackageTypeName) {
    return {
        items: elements,
        getId(item) {
            return item.id;
        },
        getDisplayName(item) {
            return item.getName();
        },
        getItemNotFoundMessage(itemId) {
            return `No "${elementTypeName}" found with id "${itemId}".`;
        },
        getNoItemsFoundMessage() {
            return `No Elements of type "${elementTypeName}" could be found. Please ensure that you have a reference to the ${relevantPackageTypeName} package and that at least one ${elementTypeName} exists in it.`;
        },
    };
}
/**
 * Dialog selection.
 * @param options For simplicity, use getPackageSelectItemOptions() or getElementSelectItemOptions()
 * @returns Selected item.
 */
async function openSelectItemDialog(options) {
    if (!options) {
        throw new Error("Options are required for 'openSelectItemDialog'.");
    }
    let items = options.items;
    if (items.length == 0) {
        await dialogService.info(options.getNoItemsFoundMessage());
        return null;
    }
    let itemId = await dialogService.lookupFromOptions(items.map(item => ({
        id: options.getId(item),
        name: options.getDisplayName(item)
    })));
    if (itemId == null) {
        await dialogService.error(options.getItemNotFoundMessage(itemId));
        return null;
    }
    let foundItem = items.filter(item => options.getId(item) === itemId)[0];
    return foundItem;
}
/// <reference path="../../typings/elementmacro.context.api.d.ts"/>
/// <reference path="attributeWithMapPath.ts" />
class ServicesConstants {
}
ServicesConstants.dtoToEntityMappingId = "942eae46-49f1-450e-9274-a92d40ac35fa"; //"01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
ServicesConstants.dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
ServicesConstants.dtoToDomainOperation = "8d1f6a8a-77c8-43a2-8e60-421559725419";
class ServicesHelper {
    static addDtoFieldsFromDomain(dto, attributes) {
        for (let key of attributes) {
            if (dto && !dto.getChildren("DTO-Field").some(x => x.getName() == ServicesHelper.getFieldFormat(key.name))) {
                let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(key.name), dto.id);
                field.typeReference.setType(key.typeId);
                if ((key.mapPath ?? []).length > 0) {
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
 * Helper class to build up source and target mapping paths for
 * advanced mapping scenarios.
 *
 * @remarks
 *
 * Source Path and Target Path is maintained separately since not all mapping scenarios are
 * straightforward.
 *
 * @example
 *
 * When to Push/Pop the Paths
 *
    let leftField = createField(...);

    mappingStore.pushSourcePath(leftField.id);
    mappingStore.pushTargetPath(rightField.id);

    let leftFieldDto = replicateDto(rightField.typeReference.getType(), folder, mappingStore);

    mappingStore.popSourcePath();
    mappingStore.popTargetPath();

    leftField.typeReference.setType(leftFieldDto.id);
 *
 * Adding mappings
 *
    function replicateDto(existingDto: MacroApi.Context.IElementApi, ...) {
        let newDto = createElement("DTO", existingDto.getName(), folder.id);
        existingDto.getChildren("DTO-Field").forEach(existingField => {
            let newField = createElement("DTO-Field", existingField.getName(), newDto.id);
            // ...
            mappingStore.addMapping(newField.id, existingField.id);
            // ...
        }
    }
 */
class MappingStore {
    constructor() {
        this.mappings = [];
        this.sourcePath = [];
        this.targetPath = [];
    }
    /**
     * Get all the recorded mapping entries
     */
    getMappings() {
        return this.mappings;
    }
    /**
     * Keep track of this element id on the source end
     * when navigating inside it's type hierarchy.
     */
    pushSourcePath(id) {
        this.sourcePath.push(id);
    }
    /**
     * Remove the last tracked element on the source path stack
     * when done navigating down its type hierarchy.
     */
    popSourcePath() {
        this.sourcePath.pop();
    }
    /**
     * Keep track of this element id on the target end
     * when navigating inside it's type hierarchy.
     */
    pushTargetPath(id) {
        this.targetPath.push(id);
    }
    /**
     * Remove the last tracked element on the target path stack
     * when done navigating down its type hierarchy.
     */
    popTargetPath() {
        this.targetPath.pop();
    }
    /**
     * Record a mapping between a source element id and target element id.
     * Target and Source path stack will be used to build up the whole path.
     */
    addMapping(sourceId, targetId) {
        this.mappings.push({
            sourcePath: this.sourcePath.concat([sourceId]),
            targetPath: this.targetPath.concat([targetId])
        });
    }
}
class ElementManager {
    constructor(innerElement, settings) {
        this.innerElement = innerElement;
        this.settings = settings;
        this.mappedElement = innerElement.getMapping()?.getElement();
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
        let existingField = this.innerElement.getChildren(this.settings.childSpecialization)
            .find(c => c.getName().toLowerCase() == ServicesHelper.formatName(name, this.settings.childType ?? "property").toLowerCase());
        let field = existingField ?? createElement(this.settings.childSpecialization, ServicesHelper.formatName(name, this.settings.childType ?? "property"), this.innerElement.id);
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
                if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
            let field = this.addChild(e.name, e.typeId);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
            if (options?.addToTop) {
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
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
function getSurrogateKeyType() {
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
        return element?.specialization === "Data Contract" ||
            element?.specialization === "Value Object" ||
            element?.specialization === "Class";
    }
    static isComplexTypeById(typeId) {
        let element = lookup(typeId);
        return DomainHelper.isComplexType(element);
    }
    static getOwningAggregate(entity) {
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
        let result = entity.getAssociations("Association")
            .filter(x => this.isAggregateRoot(x.typeReference.getType()) && isOwnedBy(x) &&
            // Let's only target collections for now as part of the nested compositional crud support
            // as one-to-one relationships are more expensive to address and possibly not going to
            // be needed.
            x.getOtherEnd().typeReference.isCollection)[0]?.typeReference.getType();
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
        if (!entity) {
            throw new Error("entity not specified");
        }
        if (!owningAggregate) {
            throw new Error("nestedCompOwner not specified");
        }
        // Use the new Associated property on the FK stereotype method for FK Attribute lookup
        let foreignKeys = [];
        for (let attr of entity.getChildren("Attribute").filter(x => x.hasStereotype("Foreign Key"))) {
            let associationId = attr.getStereotype("Foreign Key").getProperty("Association")?.getValue();
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
        let fkAssociation = attribute.getStereotype("Foreign Key")?.getProperty("Association")?.getSelected();
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
            .filter(x => !x.hasStereotype("Primary Key") &&
            !DomainHelper.legacyPartitionKey(x) &&
            (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
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
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return application.getSettings(domainSettingsId)
            ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
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
function ensureDtoFields(autoAddPrimaryKey, mappedElement, dto) {
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
        .filter(x => !x.hasStereotype("Primary Key") &&
        !legacyPartitionKey(x) &&
        (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
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
    const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
    return application.getSettings(domainSettingsId)
        ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
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
            result.selectedDomainOperationIds = dialogResult.tree?.filter((x) => x != "0") ?? [];
        }
        return result;
        function getClassAdditionalInfo(element) {
            let aggregateEntity = DomainHelper.getOwningAggregate(element);
            let prefix = aggregateEntity ? `: ${aggregateEntity.getName()}  ` : "";
            return `${prefix}(${element.getParents().map(item => item.getName()).join("/")})`;
        }
    }
    static filterClassSelection(element, options) {
        if (!(options?.allowAbstract ?? false) && element.getIsAbstract()) {
            return false;
        }
        if (element.hasStereotype("Repository")) {
            return true;
        }
        if (options?.includeOwnedRelationships != false && DomainHelper.ownerIsAggregateRoot(element)) {
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
                if (dto.getChildren("Parameter").some(x => x.getMapping()?.getElement()?.id == e.id)) {
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
        let dtoUpdated = false;
        let domainElement = mappedElement;
        let attributesWithMapPaths = CrudHelper.getAttributesWithMapPath(domainElement);
        let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
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
            .filter(x => !x.hasStereotype("Primary Key") &&
            !DomainHelper.isManagedForeignKey(x) && // essentially also an attribute set by infrastructure
            !CrudHelper.legacyPartitionKey(x) &&
            (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
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
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return application.getSettings(domainSettingsId)
            ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
    }
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
}
/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
const stringTypeId = "d384db9c-a279-45e1-801e-e4e8099625f2";
function onMapDto(element, folder, autoAddPrimaryKey = true, dtoPrefix = null, inbound = false) {
    if (element.isMapped) {
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
        .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.getMapping()?.getElement()?.specialization.startsWith("Association"));
    fields.forEach(f => {
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, f.getMapping().getElement().typeReference.getType(), dtoPrefix), f.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, targetMappingSettingId, folder, inbound);
        f.typeReference.setType(newDto.id);
    });
    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO"
        && (DomainHelper.isComplexType(x.getMapping()?.getElement()?.typeReference?.getType())));
    complexAttributes.forEach(f => {
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, f.getMapping().getElement(), dtoPrefix), f.getMapping().getElement().typeReference.getType(), false, targetMappingSettingId, folder, inbound);
        f.typeReference.setType(newDto.id);
    });
}
/// <reference path="servicesHelper.ts" />
/// <reference path="mappingStore.ts" />
/// <reference path="elementManager.ts" />
/// <reference path="../services-cqrs-crud/_common/onMapDto.ts" />
var RepositoryCrudType;
(function (RepositoryCrudType) {
    RepositoryCrudType[RepositoryCrudType["Create"] = 0] = "Create";
    RepositoryCrudType[RepositoryCrudType["Read"] = 1] = "Read";
    RepositoryCrudType[RepositoryCrudType["Update"] = 2] = "Update";
    RepositoryCrudType[RepositoryCrudType["Delete"] = 3] = "Delete";
})(RepositoryCrudType || (RepositoryCrudType = {}));
const mapToDomainOperationSettingId = "7c31c459-6229-4f10-bf13-507348cd8828";
class RepositoryServiceHelper {
    static _createService(repository, folder) {
        let serviceName = repository.getName();
        serviceName = RepositoryServiceHelper.sanitizeServiceName(serviceName);
        const existing = folder.getPackage().getChildren("Service").find(x => x.getName() == serviceName);
        if (existing) {
            return existing;
        }
        let serviceElement = createElement("Service", serviceName, folder.getPackage().id);
        return serviceElement;
    }
    static sanitizeServiceName(name) {
        name = removeSuffix(name, "Repository", "DAL");
        name += "Service";
        name = toPascalCase(name);
        return name;
    }
    static createAppServiceOperationAction(operation, folder, service, syncElement = false) {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);
        if (!service) {
            service = RepositoryServiceHelper._createService(operation.getParent(), folder);
        }
        // look up if there is an existing operation with the same name
        const existing = service.getChildren().find(x => x.getName() == operationName);
        // and return the existing one if the sync is disable (which is is by default)
        if (existing && !syncElement) {
            return existing;
        }
        let operationElement = existing ?? createElement("Operation", operationName, service.id);
        let mappingStore = new MappingStore();
        RepositoryServiceHelper.createAction(operation.getChildren("Parameter"), operationElement, false, folder, mappingStore, existing != null);
        // only add the association if not an existing operation
        if (!existing) {
            let callOp = createAssociation("Perform Invocation", operationElement.id, operation.id);
            let mapping = callOp.createAdvancedMapping(operationElement.id, operation.id);
            mapping.addMappedEnd("Invocation Mapping", [operationElement.id], [operation.id]);
            for (let entry of mappingStore.getMappings().reverse()) {
                mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
            }
        }
        if (!DomainHelper.isComplexType(operation.typeReference?.getType())) {
            operationElement.typeReference.setType(operation.typeReference.getTypeId());
            operationElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            operationElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }
        else {
            var resultDto = RepositoryServiceHelper.createRepositoryClassTypeDto(operation, operation.typeReference?.getType(), folder);
            operationElement.typeReference.setType(resultDto.id);
            operationElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            operationElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }
        return operationElement;
    }
    static createCqrsAction(operation, folder, syncElement = false) {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async", "Command", "Query");
        operationName = toPascalCase(operationName);
        let metadata = RepositoryServiceHelper.getRepositoryOperationMetadata(operation);
        let actionTypeName;
        switch (metadata.crudType) {
            default:
                actionTypeName = "Command";
                break;
            case RepositoryCrudType.Read:
                actionTypeName = "Query";
                break;
        }
        const actionName = `${operationName}${actionTypeName}`;
        // if sync is set to true, then don't return right away
        const existing = folder.getChildren().find(x => x.getName() == actionName);
        if (existing && !syncElement) {
            return existing;
        }
        const actionElement = existing ?? createElement(actionTypeName, actionName, folder.id);
        let mappingStore = new MappingStore();
        RepositoryServiceHelper.createAction(operation.getChildren("Parameter"), actionElement, true, folder, mappingStore, existing != null);
        // don't recreate the association if it the entity exists exists
        if (!existing) {
            let callOp = createAssociation("Perform Invocation", actionElement.id, operation.id);
            let mapping = callOp.createAdvancedMapping(actionElement.id, operation.id);
            mapping.addMappedEnd("Invocation Mapping", [actionElement.id], [operation.id]);
            for (let entry of mappingStore.getMappings().reverse()) {
                mapping.addMappedEnd("Data Mapping", entry.sourcePath, entry.targetPath);
            }
        }
        if (!DomainHelper.isComplexType(operation.typeReference?.getType())) {
            actionElement.typeReference.setType(operation.typeReference.getTypeId());
            actionElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            actionElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }
        else {
            var resultDto = RepositoryServiceHelper.createRepositoryClassTypeDto(operation, operation.typeReference?.getType(), folder);
            actionElement.typeReference.setType(resultDto.id);
            actionElement.typeReference.setIsCollection(operation.typeReference.getIsCollection());
            actionElement.typeReference.setIsNullable(operation.typeReference.getIsNullable());
        }
        return actionElement;
    }
    static getRepositoryOperationMetadata(operation) {
        let mappedElement = operation.getMapping()?.getElement();
        mappedElement;
        let crudType;
        let httpSettings = mappedElement?.getStereotype("Http Settings");
        let httpVerb = httpSettings?.getProperty("Verb")?.getValue();
        let httpRoute = httpSettings?.getProperty("Route")?.getValue();
        const routeParamRegex = /\{([a-zA-Z0-9_\-]+)\}/g;
        let httpRouteParams = httpRoute ? [...httpRoute.matchAll(routeParamRegex)].map(match => match[1]) : [];
        if (httpVerb) {
            switch (httpVerb.toUpperCase()) {
                case "POST":
                    crudType = RepositoryCrudType.Create;
                    break;
                case "PUT":
                    crudType = RepositoryCrudType.Update;
                    break;
                case "DELETE":
                    crudType = RepositoryCrudType.Delete;
                    break;
                case "GET":
                    crudType = RepositoryCrudType.Read;
                    break;
            }
        }
        else if (mappedElement && (mappedElement.specialization === "Command" ||
            mappedElement.specialization === "Query" ||
            mappedElement.specialization === "Operation")) {
            for (let association of mappedElement.getAssociations()) {
                switch (association.specialization) {
                    case "Create Entity Action":
                        crudType = RepositoryCrudType.Create;
                        break;
                    case "Update Entity Action":
                        crudType = RepositoryCrudType.Update;
                        break;
                    case "Delete Entity Action":
                        crudType = RepositoryCrudType.Delete;
                        break;
                    case "Query Entity Action":
                        crudType = RepositoryCrudType.Read;
                        break;
                }
            }
        }
        else if (!crudType) {
            let mappedElementNameLower = (mappedElement ? mappedElement.getName() : operation.getName()).toLocaleLowerCase();
            if (mappedElementNameLower.indexOf("create") > -1) {
                crudType = RepositoryCrudType.Create;
            }
            else if (mappedElementNameLower.indexOf("update") > -1) {
                crudType = RepositoryCrudType.Update;
            }
            else if (mappedElementNameLower.indexOf("delete") > -1) {
                crudType = RepositoryCrudType.Delete;
            }
            else if (mappedElementNameLower.indexOf("get") > -1 || mappedElementNameLower.indexOf("find") > -1) {
                crudType = RepositoryCrudType.Read;
            }
        }
        return {
            crudType: crudType,
            httpVerb: httpVerb,
            httpRoute: httpRoute,
            httpRouteParams: httpRouteParams
        };
    }
    static createAction(parameters, actionElement, flattenFieldsFromComplexTypes, folder, mappingStore, isExistingElement = false) {
        const childSpecialization = actionElement.specialization == "Operation" ? "Parameter" : "DTO-Field";
        let elementManager = new ElementManager(actionElement, { childSpecialization: childSpecialization });
        for (let repositoryField of parameters) {
            let paramRefType = repositoryField.typeReference?.getType()?.specialization;
            switch (paramRefType) {
                case "Class":
                case "Data Contract":
                case "Value Object":
                    let repositoryRefType = repositoryField.typeReference.getType();
                    if (flattenFieldsFromComplexTypes && !repositoryField.typeReference?.isCollection) {
                        mappingStore.pushTargetPath(repositoryField.id);
                        RepositoryServiceHelper.createAction(repositoryRefType.getChildren("Attribute"), elementManager.getElement(), false, folder, mappingStore, isExistingElement);
                        mappingStore.popTargetPath();
                    }
                    else {
                        let actionField = elementManager.addChild(repositoryField.getName(), null);
                        mappingStore.pushSourcePath(actionField.id);
                        mappingStore.pushTargetPath(repositoryField.id);
                        let actionDto = RepositoryServiceHelper.replicateDto(repositoryRefType, folder, mappingStore, isExistingElement);
                        mappingStore.popSourcePath();
                        mappingStore.popTargetPath();
                        actionField.typeReference.setType(actionDto.id);
                        actionField.typeReference.setIsCollection(repositoryField.typeReference.isCollection);
                        actionField.typeReference.setIsNullable(repositoryField.typeReference.isNullable);
                        if (repositoryField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                            actionField.addMetadata("endpoint-input-id", repositoryField.getMetadata("endpoint-input-id"));
                        }
                        if (repositoryField.typeReference?.isCollection) {
                            actionField.setValue(actionDto.getValue());
                            if (!isExistingElement) {
                                mappingStore.addMapping(actionField.id, repositoryField.id);
                            }
                        }
                    }
                    break;
                default:
                    // Non-Complex type
                    // if mapping directly to a class, skip over the primary keys 
                    if (repositoryField.hasStereotype("Primary Key")) {
                        continue;
                    }
                    let fieldName = repositoryField.getName();
                    if (elementManager.getElement().getChildren().some(x => x.getName() === fieldName) && !isExistingElement) {
                        let parentName = repositoryField.getParent().getName();
                        fieldName = parentName + fieldName;
                    }
                    let actionField = elementManager.addChild(fieldName, repositoryField.typeReference);
                    actionField.setValue(repositoryField.getValue());
                    if (!isExistingElement) {
                        mappingStore.addMapping(actionField.id, repositoryField.id);
                    }
                    if (repositoryField.hasMetadata("endpoint-input-id") && !actionField.hasMetadata("endpoint-input-id")) {
                        actionField.addMetadata("endpoint-input-id", repositoryField.getMetadata("endpoint-input-id"));
                    }
                    break;
            }
        }
        elementManager.collapse();
    }
    static getBaseNameForElement(owningAggregate, entity, entityIsMany) {
        // Keeping 'owningAggregate' in case we still need to use it as part of the name one day
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }
    static createRepositoryClassTypeDto(operation, entity, folder) {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        let expectedDtoName = `${operation.getName()}${baseName}Dto`;
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
                field.typeReference.setType(fk.typeId);
                if (fk.mapPath) {
                    field.setMapping(fk.mapPath);
                }
            });
        }
        ServicesHelper.addDtoFieldsFromDomain(dto, primaryKeys);
        let attributesWithMapPaths = DomainHelper.getAttributesWithMapPath(entity);
        for (var attr of attributesWithMapPaths) {
            if (dto.getChildren("DTO-Field").some(x => x.getMapping()?.getElement()?.id == attr.id)) {
                continue;
            }
            let field = createElement("DTO-Field", attr.name, dto.id);
            field.typeReference.setType(attr.typeReferenceModel);
            field.setMapping(attr.mapPath);
        }
        onMapDto(dto, folder);
        dto.collapse();
        return dto;
    }
    static replicateDto(repositoryDto, folder, mappingStore, isExistingElement = false) {
        var expectedName = `${repositoryDto.getName()}Dto`;
        // check to see if there is a DTO with the same name, and only if there is and we are working with an existing element do we update
        // this is to preserve backwards compatibility
        let existingDto = folder.getChildren().find(x => x.getName() == expectedName);
        let newDto = (existingDto && isExistingElement) ? existingDto : createElement("DTO", expectedName, folder.id);
        let elementManager = new ElementManager(newDto, { childSpecialization: "DTO-Field" });
        repositoryDto.getChildren("Attribute").forEach(repositoryField => {
            let existingField = newDto.getChildren().find(c => c.getName() == repositoryField.getName());
            //let actionField =  (existingField && isExistingElement) ? existingField :  createElement("DTO-Field", repositoryField.getName(), newDto.id);
            let actionField = (existingField && isExistingElement) ? existingField : elementManager.addChild(repositoryField.getName(), null);
            let fieldRefType = repositoryField.typeReference?.getType()?.specialization;
            switch (fieldRefType) {
                case "Class":
                case "Data Contract":
                case "Value Object":
                    ``;
                    // Complex type
                    mappingStore.pushSourcePath(actionField.id);
                    mappingStore.pushTargetPath(repositoryField.id);
                    let nestedDto = RepositoryServiceHelper.replicateDto(repositoryField.typeReference.getType(), folder, mappingStore, isExistingElement);
                    mappingStore.popSourcePath();
                    mappingStore.popTargetPath();
                    actionField.typeReference.setType(nestedDto.id);
                    if (repositoryField.typeReference?.isCollection) {
                        actionField.setValue(nestedDto.getValue());
                        mappingStore.addMapping(actionField.id, repositoryField.id);
                    }
                    break;
                default:
                    // Non-Complex type
                    // if mapping directly to a class, skip over the primary keys 
                    if (repositoryField.hasStereotype("Primary Key")) {
                        break;
                    }
                    actionField.typeReference.setType(repositoryField.typeReference.getTypeId());
                    actionField.setValue(repositoryField.getValue());
                    mappingStore.addMapping(actionField.id, repositoryField.id);
                    break;
            }
            actionField.typeReference.setIsCollection(repositoryField.typeReference.isCollection);
            actionField.typeReference.setIsNullable(repositoryField.typeReference.isNullable);
        });
        return newDto;
    }
}
/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/repositoryServiceHelper.ts" />
/// <reference path="common.ts" />
async function createCQRSService(repositoryOperation, diagram) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    }
    else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }
    const repository = repositoryOperation.getParent();
    const folderName = pluralize(removeSuffix(repository.getName(), "Repository", "DAL"));
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);
    const requestElement = RepositoryServiceHelper.createCqrsAction(repositoryOperation, folder, true);
    if (diagram == null) {
        diagram = await getOrCreateDiagram(folder, repositoryOperation, "CQRS Creation Options");
    }
    if (diagram == null) {
        return;
    }
    //Since we're adding a single new element on the diagram, it may not be positioned below the last created one.
    let lastActionVisual = null;
    for (let action of folder.getChildren("Command").concat(folder.getChildren("Query"))) {
        if (diagram.isVisual(action.id)) {
            var actionElement = diagram.getVisual(action.id);
            if (!lastActionVisual || actionElement.getPosition().y > lastActionVisual.getPosition().y) {
                lastActionVisual = actionElement;
            }
        }
    }
    let newPosition = null;
    let repoElement = diagram.getVisual(repository.id);
    // This is an attempt to reposition the newly created elements due to the lack of
    // directly manipulating the visuals on the diagram but it ends up skewing diagonally.
    if (lastActionVisual) {
        newPosition = {
            x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
            y: lastActionVisual.getPosition().y + (lastActionVisual.getSize().height * 1.5)
        };
    }
    else {
        if (diagram.isVisual(repository.id)) {
            newPosition = {
                x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
                y: repoElement.getPosition().y
            };
        }
    }
    diagram.layoutVisuals(folder, newPosition, true);
    diagram.getVisual(requestElement.id)?.select();
}
/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/repositoryServiceHelper.ts" />
/// <reference path="common.ts" />
async function createTraditionalService(repositoryOperation, diagram) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    }
    else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }
    const repository = repositoryOperation.getParent();
    const folderName = pluralize(removeSuffix(repository.getName(), "Repository", "DAL"));
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);
    let newOperation = RepositoryServiceHelper.createAppServiceOperationAction(repositoryOperation, folder, null, true);
    if (diagram == null) {
        diagram = await getOrCreateDiagram(folder, repositoryOperation, "Traditional Service Creation Options");
    }
    if (diagram == null) {
        return;
    }
    let newPosition = null;
    let repoElement = diagram.getVisual(repository.id);
    // This is an attempt to reposition the newly created elements due to the lack of
    // directly manipulating the visuals on the diagram but it ends up skewing diagonally.
    if (repoElement) {
        newPosition = {
            x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
            y: repoElement.getPosition().y + (repoElement.getSize().height / 2)
        };
    }
    diagram.layoutVisuals(newOperation.getParent(), newPosition, true);
    diagram.getVisual(newOperation.id)?.select();
}
/// <reference path="createCqrs.ts" />
const RepositoryOperationApi = {
    createCQRSService,
    createTraditionalService
};
