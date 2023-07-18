/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="../../../common/applyAttributeNamingConvention.ts" />
/// <reference path="../_common/constants.ts" />

function updateForeignKeys(association: MacroApi.Context.IAssociationApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    let sourceEnd = association.getOtherEnd().typeReference;
    if (!sourceEnd.getType().getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    let sourceType = association.getOtherEnd().typeReference.getType();
    let targetType = association.typeReference.getType();

    if (sourceType?.specialization !== "Class" || targetType?.specialization !== "Class") {
        return;
    }

    if (requiresForeignKey(association)) {
        updateForeignKeyAttribute(sourceType, targetType, association, association.id);
        return;
    }

    sourceType.getChildren()
        .filter(x => x.getMetadata("association") == association.id)
        .forEach(x => {
            x.setMetadata(isBeingDeletedByScript, "true");
            x.delete();
        });

    function updateForeignKeyAttribute(
        startingEndType: MacroApi.Context.IElementApi,
        destinationEndType: MacroApi.Context.IElementApi,
        associationEnd: MacroApi.Context.IAssociationApi,
        associationId: string
    ): void {
        const pkAttributes = getPrimaryKeys(destinationEndType);

        pkAttributes.forEach((pk, index) => {
            let fkAttribute = startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId)[index] ??
                createElement("Attribute", "", startingEndType.id);

            // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
            let fkNameToUse = `${toCamelCase(associationEnd.getName())}${toPascalCase(pk.name)}`;
            if (fkAttribute.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
                if (!fkAttribute.hasMetadata("fk-original-name") || (fkAttribute.getMetadata("fk-original-name") == fkAttribute.getName())) {
                    fkAttribute.setName(fkNameToUse);
                    fkAttribute.setMetadata("fk-original-name", fkAttribute.getName());
                }
            }

            fkAttribute.setMetadata("association", associationId);
            fkAttribute.setMetadata(isManagedKey, "true");

            let fkStereotype = fkAttribute.getStereotype(foreignKeyStereotypeId);
            if (fkStereotype == null) {
                fkAttribute.addStereotype(foreignKeyStereotypeId);
                fkStereotype = fkAttribute.getStereotype(foreignKeyStereotypeId);
            }
            fkStereotype.getProperty("Association").setValue(association.isTargetEnd() ? association.id : association.getOtherEnd().id);

            fkAttribute.typeReference.setType(pk.typeId);
            fkAttribute.typeReference.setIsNullable(associationEnd.typeReference.isNullable);
        });

        startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId).forEach((attr, index) => {
            if (index >= pkAttributes.length) {
                attr.setMetadata(isBeingDeletedByScript, "true");
                attr.delete();
            }
        });

        if (destinationEndType.id !== startingEndType.id) {
            destinationEndType.getChildren()
                .filter(x => x.getMetadata("association") == associationId)
                .forEach(x => {
                    x.setMetadata(isBeingDeletedByScript, "true");
                    x.delete();
                });
        }
    }

    function requiresForeignKey(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return isManyToVariantsOfOne(associationEnd) || isSelfReferencingZeroToOne(associationEnd);
    }

    function isManyToVariantsOfOne(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
    }

    function isSelfReferencingZeroToOne(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
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
