/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="../../../common/applyAttributeNamingConvention.ts" />
/// <reference path="../_common/constants.ts" />

function updateForeignKeys(selectedEnd: MacroApi.Context.IAssociationReadOnlyApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    let theOtherEnd = selectedEnd.getOtherEnd();
    if (!theOtherEnd.typeReference.getType().getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    let theOtherEndType = theOtherEnd.typeReference.getType();
    let theSelectedEndType = selectedEnd.typeReference.getType();

    if (theOtherEndType?.specialization !== "Class" || theSelectedEndType?.specialization !== "Class") {
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

    function updateForeignKeyAttribute(
        selectedEnd: MacroApi.Context.IAssociationReadOnlyApi,
        theOtherEndType: MacroApi.Context.IElementApi,
        theSelectedEndType: MacroApi.Context.IElementApi,
        targetEndId: string
    ): void {
        const pkAttributesOfSelectedEnd = getPrimaryKeys(theSelectedEndType);

        pkAttributesOfSelectedEnd.forEach((primaryKeyOfSelectedEnd, index) => {
            let fkAttributeOfOtherEnd = theOtherEndType.getChildren().filter(x => x.getMetadata(metadataKey.association) == targetEndId)[index] ??
                createElement("Attribute", "", theOtherEndType.id);

            // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
            let fkNameToUse = getForeignKeyName(
                selectedEnd, 
                theSelectedEndType,
                primaryKeyOfSelectedEnd, 
                fkAttributeOfOtherEnd.getName() !== "" ? fkAttributeOfOtherEnd : null);

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

    interface IPrimaryKey {
        name: string;
        typeId: string;
    }

    // If you change the logic around determining FK Name make sure to update the SQL Importer to have the same logic
    function getForeignKeyName(
        selectedEnd: MacroApi.Context.IAssociationReadOnlyApi,
        theSelectedEndType: MacroApi.Context.IElementApi,
        primaryKeyOfSelectedEnd: IPrimaryKey,
        fkAttributeOfOtherEnd? : MacroApi.Context.IElementApi)
        : string {
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

    function requiresForeignKey(associationEnd: MacroApi.Context.IAssociationReadOnlyApi): boolean {
        const isSelfReferencingWithoutManyToMany = () => 
            associationEnd.typeReference.getTypeId() === associationEnd.getOtherEnd().typeReference.getTypeId() &&
            (!associationEnd.typeReference.isCollection || !associationEnd.getOtherEnd().typeReference.isCollection);
        const isManyToVariantsOfOne = () =>
            !associationEnd.typeReference.isCollection &&
            associationEnd.getOtherEnd().typeReference.isCollection;
        const isSelfReferencingZeroToOne = () =>
            !associationEnd.typeReference.isCollection &&
            associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
        const isAggregationalOneToOne = () =>
            associationEnd.isTargetEnd() &&
            !associationEnd.typeReference.isCollection &&
            !associationEnd.getOtherEnd().typeReference.isCollection &&
            associationEnd.getOtherEnd().typeReference.isNullable;

        return isSelfReferencingWithoutManyToMany() || isManyToVariantsOfOne() || isSelfReferencingZeroToOne() || isAggregationalOneToOne();
    }

    function getPrimaryKeys(element: MacroApi.Context.IElementApi): IPrimaryKey[] {
        let currentClass = element;
        while (currentClass?.specialization === "Class") {
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
            if (currentClass?.id === element?.id) {
                console.error(`Could not compute possible foreign keys as "${element.getName()}" [${element.id}] has cyclic inheritance.`);
                return [createImplicitPrimaryKey()];
            }
        }

        return [createImplicitPrimaryKey()];

        function createImplicitPrimaryKey(): IPrimaryKey {
            return {
                name: applyAttributeNamingConvention("Id"),
                typeId: getSurrogateKeyType()
            };
        }
    }
}
