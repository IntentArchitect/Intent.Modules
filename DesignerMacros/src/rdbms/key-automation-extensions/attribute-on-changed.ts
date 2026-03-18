function executeAttributeOnChanged(attributeElement: MacroApi.Context.IElementApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        attributeElement.getParent().getMetadata(metadataKey.autoManageKeys) === "false" ||
        attributeElement.getPackage().specialization !== "Domain Package" ||
        !attributeElement.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    if (attributeElement.getName().toLocaleLowerCase() == "id" &&
        attributeElement.hasStereotype("Primary Key") &&
        attributeElement.typeReference.getType()) {

        if (attributeElement.typeReference.getType()?.id != getSurrogateKeyType()) {
            attributeElement.setMetadata(metadataKey.isManagedKey, "false");
        } else {
            attributeElement.setMetadata(metadataKey.isManagedKey, "true");
        }
    }

    let associationTarget = attributeElement.getStereotype(foreignKeyStereotypeId)?.getProperty(foreignKeyStereotypeAssociationProperty)?.getValue() as MacroApi.Context.IElementApi;

    if (associationTarget && attributeElement.getMetadata(metadataKey.association) != associationTarget.id) {
        attributeElement.setMetadata(metadataKey.association, associationTarget.id);
    }
}