function executeAttributeOnDeleted(attributeElement: MacroApi.Context.IElementApi): void {
    if (attributeElement.hasMetadata(metadataKey.isBeingDeletedByScript) ||
        !attributeElement.hasMetadata(metadataKey.isManagedKey) ||
        !attributeElement.getPackage().hasStereotype(relationalDatabaseId)
    ) {
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