/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    setDatabaseParadigm();
    setupForeignKeyAttributes();
}

function setDatabaseParadigm() {
    if (!element.hasMetadata("database-paradigm-selected") &&
        !element.hasStereotype(relationalDatabaseId) &&
        !element.hasStereotype("Document Database")
    ) {
        element.addStereotype(relationalDatabaseId);
    }

    element.setMetadata("database-paradigm-selected", "true");
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

execute();