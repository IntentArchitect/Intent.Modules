function executeClassOnChanged(classElement: MacroApi.Context.IElementApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        !classElement.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    updatePrimaryKeys(classElement);
    updateForeignKeysAndForDerived(classElement);

    function updateForeignKeysAndForDerived(forElement: MacroApi.Context.IElementApi): void {
        for (const association of forElement.getAssociations("Association")) {
            updateForeignKeys(association);
            updateForeignKeys(association.getOtherEnd());
        }

        var associationIds = forElement.getAssociations("Association")
            .map(thisEnd => thisEnd.isTargetEnd() ? thisEnd.id : thisEnd.getOtherEnd().id);
        var fkAttributesToDelete = forElement.getChildren("Attribute")
            .filter(association =>
                association.hasStereotype(foreignKeyStereotypeId) &&
                association.hasMetadata(metadataKey.association) &&
                !associationIds.some(id => id === association.getMetadata(metadataKey.association)))
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