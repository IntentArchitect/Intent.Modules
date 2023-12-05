/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="../../../common/applyAttributeNamingConvention.ts" />
/// <reference path="../_common/constants.ts" />

function updateForeignKeys(thisEnd: MacroApi.Context.IAssociationApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    let otherEnd = thisEnd.getOtherEnd();
    if (!otherEnd.typeReference.getType().getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    let thisEndType = thisEnd.getOtherEnd().typeReference.getType();
    let otherEndType = thisEnd.typeReference.getType();

    if (thisEndType?.specialization !== "Class" || otherEndType?.specialization !== "Class") {
        return;
    }

    const targetEndId = thisEnd.isTargetEnd() ? thisEnd.id : otherEnd.id;

    if (requiresForeignKey(thisEnd)) {
        updateForeignKeyAttribute(thisEnd, thisEndType, otherEndType, targetEndId);
        return;
    }

    thisEndType.getChildren()
        .filter(x => x.getMetadata(metadataKey.association) == targetEndId)
        .forEach(x => {
            x.setMetadata(metadataKey.isBeingDeletedByScript, "true");
            x.delete();
        });

    function updateForeignKeyAttribute(
        thisEnd: MacroApi.Context.IAssociationApi,
        thisEndType: MacroApi.Context.IElementApi,
        otherEndType: MacroApi.Context.IElementApi,
        targetEndId: string
    ): void {
        const pkAttributes = getPrimaryKeys(otherEndType);

        pkAttributes.forEach((pk, index) => {
            let fkAttribute = thisEndType.getChildren().filter(x => x.getMetadata(metadataKey.association) == targetEndId)[index] ??
                createElement("Attribute", "", thisEndType.id);

            // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
            let fkNameToUse = `${toCamelCase(thisEnd.getName())}${toPascalCase(pk.name)}`;
            if (fkAttribute.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
                if (!fkAttribute.hasMetadata(metadataKey.fkOriginalName) || (fkAttribute.getMetadata(metadataKey.fkOriginalName) == fkAttribute.getName())) {
                    fkAttribute.setName(fkNameToUse);
                    fkAttribute.setMetadata(metadataKey.fkOriginalName, fkAttribute.getName());
                }
            }

            fkAttribute.setMetadata(metadataKey.association, targetEndId);
            fkAttribute.setMetadata(metadataKey.isManagedKey, "true");

            let fkStereotype = fkAttribute.getStereotype(foreignKeyStereotypeId);
            if (fkStereotype == null) {
                fkAttribute.addStereotype(foreignKeyStereotypeId);
                fkStereotype = fkAttribute.getStereotype(foreignKeyStereotypeId);
            }
            fkStereotype.getProperty(foreignKeyStereotypeAssociationProperty).setValue(thisEnd.isTargetEnd() ? thisEnd.id : thisEnd.getOtherEnd().id);

            let isSelfReferencing = () => thisEnd.typeReference.getTypeId() === thisEnd.getOtherEnd().typeReference.getTypeId();

            fkAttribute.typeReference.setType(pk.typeId);
            if (thisEnd.isTargetEnd() || !isSelfReferencing()) {
                fkAttribute.typeReference.setIsNullable(thisEnd.typeReference.isNullable);
            }
        });

        thisEndType.getChildren().filter(x => x.getMetadata(metadataKey.association) == targetEndId).forEach((attr, index) => {
            if (index >= pkAttributes.length) {
                attr.setMetadata(metadataKey.isBeingDeletedByScript, "true");
                attr.delete();
            }
        });

        if (otherEndType.id !== thisEndType.id) {
            otherEndType.getChildren()
                .filter(x => x.getMetadata(metadataKey.association) == targetEndId)
                .forEach(x => {
                    x.setMetadata(metadataKey.isBeingDeletedByScript, "true");
                    x.delete();
                });
        }
    }

    function requiresForeignKey(associationEnd: MacroApi.Context.IAssociationApi): boolean {
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

    function getPrimaryKeys(element: MacroApi.Context.IElementApi): { name: string, typeId: string }[] {
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

        function createImplicitPrimaryKey(): { name: string, typeId: string } {
            return {
                name: applyAttributeNamingConvention("Id"),
                typeId: getSurrogateKeyType()
            };
        }
    }
}
