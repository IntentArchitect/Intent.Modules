function executeAttributeOnChanged(attributeElement) {
    var _a, _b, _c, _d, _e;
    if (((_b = (_a = application === null || application === void 0 ? void 0 : application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Creation Mode")) === null || _b === void 0 ? void 0 : _b.value) != "explicit" ||
        attributeElement.getParent().getMetadata(metadataKey.autoManageKeys) === "false" ||
        attributeElement.getPackage().specialization !== "Domain Package" ||
        !attributeElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    if (attributeElement.getName().toLocaleLowerCase() == "id" &&
        attributeElement.hasStereotype("Primary Key") &&
        attributeElement.typeReference.getType()) {
        if (((_c = attributeElement.typeReference.getType()) === null || _c === void 0 ? void 0 : _c.id) != getSurrogateKeyType()) {
            attributeElement.setMetadata(metadataKey.isManagedKey, "false");
        }
        else {
            attributeElement.setMetadata(metadataKey.isManagedKey, "true");
        }
    }
    let associationTarget = (_e = (_d = attributeElement.getStereotype(foreignKeyStereotypeId)) === null || _d === void 0 ? void 0 : _d.getProperty(foreignKeyStereotypeAssociationProperty)) === null || _e === void 0 ? void 0 : _e.getValue();
    if (associationTarget && attributeElement.getMetadata(metadataKey.association) != associationTarget.id) {
        attributeElement.setMetadata(metadataKey.association, associationTarget.id);
    }
}
function executeAttributeOnDeleted(attributeElement) {
    if (attributeElement.hasMetadata(metadataKey.isBeingDeletedByScript) ||
        !attributeElement.hasMetadata(metadataKey.isManagedKey) ||
        !attributeElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    const classElement = attributeElement.getParent();
    if (attributeElement.hasStereotype(primaryKeyStereotypeId)) {
        classElement.setMetadata(metadataKey.autoManageKeys, "false");
    }
    for (const association of classElement.getAssociations("Association")) {
        updateForeignKeys(association.getOtherEnd());
    }
}
function executeClassAutoManageKeys(classElement) {
    var _a, _b;
    if (!classElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    classElement.removeMetadata(metadataKey.autoManageKeys);
    if (((_b = (_a = application === null || application === void 0 ? void 0 : application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Creation Mode")) === null || _b === void 0 ? void 0 : _b.value) != "explicit") {
        return;
    }
    updatePrimaryKeys(classElement);
    for (const association of classElement.getAssociations("Association")) {
        updateForeignKeys(association);
        updateForeignKeys(association.getOtherEnd());
    }
}
function executeClassOnChanged(classElement) {
    var _a, _b;
    if (((_b = (_a = application === null || application === void 0 ? void 0 : application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Creation Mode")) === null || _b === void 0 ? void 0 : _b.value) != "explicit" ||
        !classElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    updatePrimaryKeys(classElement);
    updateForeignKeysAndForDerived(classElement);
    function updateForeignKeysAndForDerived(forElement) {
        for (const association of forElement.getAssociations("Association")) {
            updateForeignKeys(association);
            updateForeignKeys(association.getOtherEnd());
        }
        var associationIds = forElement.getAssociations("Association")
            .map(thisEnd => thisEnd.isTargetEnd() ? thisEnd.id : thisEnd.getOtherEnd().id);
        var fkAttributesToDelete = forElement.getChildren("Attribute")
            .filter(association => association.hasStereotype(foreignKeyStereotypeId) &&
            association.hasMetadata(metadataKey.association) &&
            !associationIds.some(id => id === association.getMetadata(metadataKey.association)));
        for (const fkAttribute of fkAttributesToDelete) {
            fkAttribute.setMetadata(metadataKey.isBeingDeletedByScript, "true");
            fkAttribute.delete();
        }
        var derivedTypes = forElement.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());
        for (const derivedType of derivedTypes) {
            updateForeignKeysAndForDerived(derivedType);
        }
    }
}
function executeClassOnCreated(classElement) {
    var _a, _b;
    if (((_b = (_a = application === null || application === void 0 ? void 0 : application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Creation Mode")) === null || _b === void 0 ? void 0 : _b.value) != "explicit" ||
        !classElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    updatePrimaryKeys(classElement);
}
function executePackageOnLoaded(packageElement) {
    setDatabaseParadigm(packageElement);
    setupForeignKeyAttributes();
    function setDatabaseParadigm(packageElement) {
        if (!packageElement.hasMetadata("database-paradigm-selected") &&
            !packageElement.hasStereotype(relationalDatabaseId) &&
            !packageElement.hasStereotype("Document Database")) {
            packageElement.addStereotype(relationalDatabaseId);
        }
        packageElement.setMetadata("database-paradigm-selected", "true");
    }
    function setupForeignKeyAttributes() {
        lookupTypesOf("Attribute").forEach(attr => {
            processSingleForeignKeyAttribute(attr);
        });
    }
    function processSingleForeignKeyAttribute(attr) {
        var _a, _b;
        if (!attr.hasMetadata(metadataKey.association) ||
            attr.getPackage().specialization !== "Domain Package" ||
            !attr.getPackage().hasStereotype(relationalDatabaseId)) {
            return;
        }
        const associationTarget = (_b = (_a = attr.getStereotype(foreignKeyStereotypeId)) === null || _a === void 0 ? void 0 : _a.getProperty(foreignKeyStereotypeAssociationProperty)) === null || _b === void 0 ? void 0 : _b.getValue();
        if (associationTarget == null) {
            if (!attr.hasStereotype(foreignKeyStereotypeId)) {
                attr.addStereotype(foreignKeyStereotypeId);
            }
            const associationId = attr.getMetadata(metadataKey.association);
            attr
                .getStereotype(foreignKeyStereotypeId)
                .getProperty(foreignKeyStereotypeAssociationProperty)
                .setValue(associationId);
        }
        if (!attr.hasMetadata(metadataKey.fkOriginalName)) {
            attr.setMetadata(metadataKey.fkOriginalName, attr.getName());
        }
    }
}
const primaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
const relationalDatabaseId = "51a7bcf5-0eb9-4c9a-855e-3ead1048729c";
const foreignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";
const foreignKeyStereotypeAssociationProperty = "Association";
const metadataKey = {
    association: "association",
    autoManageKeys: "auto-manage-keys",
    fkOriginalName: "fk-original-name",
    isBeingDeletedByScript: "is-being-deleted-by-script",
    isManagedKey: "is-managed-key",
};
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
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/**
 * Applies camelCasing or PascalCasing naming convention according to the setting configured for the current application.
 */
function applyAttributeNamingConvention(str) {
    var _a, _b, _c;
    let convention = (_c = (_b = (_a = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
    switch (convention) {
        case "pascal-case":
            return toPascalCase(str);
        case "camel-case":
            return toCamelCase(str);
    }
    return str;
}
/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="../../../common/applyAttributeNamingConvention.ts" />
/// <reference path="../_common/constants.ts" />
function updateForeignKeys(selectedEnd) {
    var _a, _b;
    if (((_b = (_a = application === null || application === void 0 ? void 0 : application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Creation Mode")) === null || _b === void 0 ? void 0 : _b.value) != "explicit") {
        return;
    }
    let theOtherEnd = selectedEnd.getOtherEnd();
    if (!theOtherEnd.typeReference.getType().getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }
    let theOtherEndType = theOtherEnd.typeReference.getType();
    let theSelectedEndType = selectedEnd.typeReference.getType();
    if ((theOtherEndType === null || theOtherEndType === void 0 ? void 0 : theOtherEndType.specialization) !== "Class" || (theSelectedEndType === null || theSelectedEndType === void 0 ? void 0 : theSelectedEndType.specialization) !== "Class") {
        return;
    }
    const targetEndId = selectedEnd.isTargetEnd() ? selectedEnd.id : theOtherEnd.id;
    if (requiresForeignKey(selectedEnd)) {
        updateForeignKeyAttribute(selectedEnd, theOtherEndType, theSelectedEndType, targetEndId);
        return;
    }
    theOtherEndType.getChildren()
        .filter(x => x.getMetadata(metadataKey.association) == targetEndId)
        .forEach(x => {
        x.setMetadata(metadataKey.isBeingDeletedByScript, "true");
        x.delete();
    });
    return;
    function updateForeignKeyAttribute(selectedEnd, theOtherEndType, theSelectedEndType, targetEndId) {
        const pkAttributesOfSelectedEnd = getPrimaryKeys(theSelectedEndType);
        pkAttributesOfSelectedEnd.forEach((primaryKeyOfSelectedEnd, index) => {
            let fkAttributeOfOtherEnd = theOtherEndType.getChildren().filter(x => x.getMetadata(metadataKey.association) == targetEndId)[index];
            if (fkAttributeOfOtherEnd == null) {
                const expectedFkName = getForeignKeyName(selectedEnd, theSelectedEndType, primaryKeyOfSelectedEnd, undefined);
                fkAttributeOfOtherEnd = theOtherEndType.getChildren("Attribute")
                    .find(x => x.getName().toLocaleLowerCase() === expectedFkName.toLocaleLowerCase()
                    && !x.getMetadata(metadataKey.association));
            }
            fkAttributeOfOtherEnd !== null && fkAttributeOfOtherEnd !== void 0 ? fkAttributeOfOtherEnd : (fkAttributeOfOtherEnd = createElement("Attribute", "", theOtherEndType.id));
            // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
            let fkNameToUse = getForeignKeyName(selectedEnd, theSelectedEndType, primaryKeyOfSelectedEnd, fkAttributeOfOtherEnd.getName() !== "" ? fkAttributeOfOtherEnd : null);
            if (fkAttributeOfOtherEnd.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
                if (!fkAttributeOfOtherEnd.hasMetadata(metadataKey.fkOriginalName) || (fkAttributeOfOtherEnd.getMetadata(metadataKey.fkOriginalName) == fkAttributeOfOtherEnd.getName())) {
                    fkAttributeOfOtherEnd.setName(fkNameToUse, false);
                    fkAttributeOfOtherEnd.setMetadata(metadataKey.fkOriginalName, fkAttributeOfOtherEnd.getName());
                }
            }
            fkAttributeOfOtherEnd.setMetadata(metadataKey.association, targetEndId);
            fkAttributeOfOtherEnd.setMetadata(metadataKey.isManagedKey, "true");
            let fkStereotype = fkAttributeOfOtherEnd.getStereotype(foreignKeyStereotypeId);
            if (fkStereotype == null) {
                fkAttributeOfOtherEnd.addStereotype(foreignKeyStereotypeId);
                fkStereotype = fkAttributeOfOtherEnd.getStereotype(foreignKeyStereotypeId);
            }
            fkStereotype.getProperty(foreignKeyStereotypeAssociationProperty).setValue(selectedEnd.isTargetEnd() ? selectedEnd.id : selectedEnd.getOtherEnd().id);
            let isSelfReferencing = () => selectedEnd.typeReference.getTypeId() === selectedEnd.getOtherEnd().typeReference.getTypeId();
            fkAttributeOfOtherEnd.typeReference.setType(primaryKeyOfSelectedEnd.typeId);
            if (selectedEnd.isTargetEnd() || !isSelfReferencing()) {
                fkAttributeOfOtherEnd.typeReference.setIsNullable(selectedEnd.typeReference.isNullable);
            }
        });
        theOtherEndType.getChildren().filter(x => x.getMetadata(metadataKey.association) == targetEndId).forEach((attr, index) => {
            if (index >= pkAttributesOfSelectedEnd.length) {
                attr.setMetadata(metadataKey.isBeingDeletedByScript, "true");
                attr.delete();
            }
        });
        if (theSelectedEndType.id !== theOtherEndType.id) {
            theSelectedEndType.getChildren()
                .filter(x => x.getMetadata(metadataKey.association) == targetEndId)
                .forEach(x => {
                x.setMetadata(metadataKey.isBeingDeletedByScript, "true");
                x.delete();
            });
        }
    }
    // If you change the logic around determining FK Name make sure to update the SQL Importer to have the same logic
    function getForeignKeyName(selectedEnd, theSelectedEndType, primaryKeyOfSelectedEnd, fkAttributeOfOtherEnd) {
        // If the FK already exists, just use it.
        if (fkAttributeOfOtherEnd != null) {
            return fkAttributeOfOtherEnd.getName();
        }
        // If Association name and Selected Type name is the same and the selected-end type is in the PK name
        if (selectedEnd.getName().toLocaleLowerCase() === theSelectedEndType.getName().toLocaleLowerCase() &&
            primaryKeyOfSelectedEnd.name.toLocaleLowerCase().includes(theSelectedEndType.getName().toLocaleLowerCase())) {
            return toPascalCase(primaryKeyOfSelectedEnd.name);
        }
        // If the Association name is composed of a supposed PK name and the selected-end type name
        // then use the association name without the selected-end type name
        if (selectedEnd.getName().includes(theSelectedEndType.getName(), 1)) {
            return toPascalCase(selectedEnd.getName().replace(theSelectedEndType.getName(), ""));
        }
        return `${toCamelCase(selectedEnd.getName())}${toPascalCase(primaryKeyOfSelectedEnd.name)}`;
    }
    function requiresForeignKey(associationEnd) {
        const isSelfReferencingWithoutManyToMany = () => associationEnd.typeReference.getTypeId() === associationEnd.getOtherEnd().typeReference.getTypeId() &&
            (!associationEnd.typeReference.isCollection || !associationEnd.getOtherEnd().typeReference.isCollection);
        const isManyToVariantsOfOne = () => !associationEnd.typeReference.isCollection &&
            associationEnd.getOtherEnd().typeReference.isCollection;
        const isSelfReferencingZeroToOne = () => !associationEnd.typeReference.isCollection &&
            associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
        const isAggregationalOneToOne = () => associationEnd.isTargetEnd() &&
            !associationEnd.typeReference.isCollection &&
            !associationEnd.getOtherEnd().typeReference.isCollection &&
            associationEnd.getOtherEnd().typeReference.isNullable;
        return isSelfReferencingWithoutManyToMany() || isManyToVariantsOfOne() || isSelfReferencingZeroToOne() || isAggregationalOneToOne();
    }
    function getPrimaryKeys(element) {
        let currentClass = element;
        while ((currentClass === null || currentClass === void 0 ? void 0 : currentClass.specialization) === "Class") {
            const pkAttributes = currentClass.getChildren("Attribute").filter(x => x.hasStereotype(primaryKeyStereotypeId));
            if (pkAttributes.length > 0) {
                return pkAttributes.map(x => ({ name: x.getName(), typeId: x.typeReference.getTypeId() }));
            }
            const derivedTypes = currentClass
                .getAssociations("Generalization")
                .filter(generalization => generalization.isTargetEnd())
                .map(generalization => generalization.typeReference.getType());
            if (derivedTypes.length > 1) {
                console.error(`Could not compute possible foreign keys as "${currentClass.getName()}" [${currentClass.id}] is derived from more than one class.`);
                return [createImplicitPrimaryKey()];
            }
            currentClass = derivedTypes[0];
            if ((currentClass === null || currentClass === void 0 ? void 0 : currentClass.id) === (element === null || element === void 0 ? void 0 : element.id)) {
                console.error(`Could not compute possible foreign keys as "${element.getName()}" [${element.id}] has cyclic inheritance.`);
                return [createImplicitPrimaryKey()];
            }
        }
        return [createImplicitPrimaryKey()];
        function createImplicitPrimaryKey() {
            return {
                name: applyAttributeNamingConvention("Id"),
                typeId: getSurrogateKeyType()
            };
        }
    }
}
/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="constants.ts" />
function updatePrimaryKeys(element, visitedElementIds = new Set()) {
    if (!element || visitedElementIds.has(element.id)) {
        return;
    }
    visitedElementIds.add(element.id);
    if (element.getMetadata(metadataKey.autoManageKeys) === "false" ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)) {
        updateDerivedTypePks(element);
        return;
    }
    const pkAttributes = element.getChildren("Attribute").filter(x => x.hasStereotype(primaryKeyStereotypeId));
    if (derivedFromTypeHasPk(element, new Set())) {
        for (const pkAttribute of pkAttributes.filter(x => x.getMetadata(metadataKey.isManagedKey) === "true")) {
            pkAttribute.setMetadata(metadataKey.isBeingDeletedByScript, "true");
            pkAttribute.delete();
        }
        updateDerivedTypePks(element);
        return;
    }
    if (pkAttributes.length > 0) {
        updateDerivedTypePks(element);
        return;
    }
    const pkAttribute = createElement("Attribute", "id", element.id);
    pkAttribute.setOrder(0);
    pkAttribute.typeReference.setType(getSurrogateKeyType());
    pkAttribute.addStereotype(primaryKeyStereotypeId);
    pkAttribute.setMetadata(metadataKey.isManagedKey, "true");
    function derivedFromTypeHasPk(element, visitedBaseTypeIds) {
        if (!element || visitedBaseTypeIds.has(element.id)) {
            return false;
        }
        visitedBaseTypeIds.add(element.id);
        return element.getAssociations("Generalization")
            .some(generalization => {
            if (!generalization.isTargetEnd()) {
                return false;
            }
            const baseType = generalization.typeReference.getType();
            if (baseType.getChildren("Attribute").some(attribute => attribute.hasStereotype(primaryKeyStereotypeId))) {
                return true;
            }
            return derivedFromTypeHasPk(baseType, visitedBaseTypeIds);
        });
    }
    function updateDerivedTypePks(element) {
        var derivedTypes = element.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());
        for (const derivedType of derivedTypes) {
            updatePrimaryKeys(derivedType, visitedElementIds);
        }
    }
}
