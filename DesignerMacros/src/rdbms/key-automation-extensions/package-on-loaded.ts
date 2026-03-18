function executePackageOnLoaded(packageElement: MacroApi.Context.IElementApi): void {
    setDatabaseParadigm(packageElement);
    setupForeignKeyAttributes();

    function setDatabaseParadigm(packageElement: MacroApi.Context.IElementApi) {
        if (!packageElement.hasMetadata("database-paradigm-selected") &&
            !packageElement.hasStereotype(relationalDatabaseId) &&
            !packageElement.hasStereotype("Document Database")
        ) {
            packageElement.addStereotype(relationalDatabaseId);
        }

        packageElement.setMetadata("database-paradigm-selected", "true");
    }


    function setupForeignKeyAttributes(): void {
        lookupTypesOf("Attribute").forEach(attr => {
            processSingleForeignKeyAttribute(attr);
        });
    }

    function processSingleForeignKeyAttribute(attr: MacroApi.Context.IElementApi) {
        if (!attr.hasMetadata(metadataKey.association) ||
            attr.getPackage().specialization !== "Domain Package" ||
            !attr.getPackage().hasStereotype(relationalDatabaseId)
        ) {
            return;
        }

        const associationTarget = attr.getStereotype(foreignKeyStereotypeId)?.getProperty(foreignKeyStereotypeAssociationProperty)?.getValue();
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