function executeClassAutoManageKeys(classElement: MacroApi.Context.IElementApi): void {
    if (!classElement.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    classElement.removeMetadata(metadataKey.autoManageKeys);

    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    updatePrimaryKeys(classElement);
    for (const association of classElement.getAssociations("Association")) {
        updateForeignKeys(association);
        updateForeignKeys(association.getOtherEnd());
    }
}