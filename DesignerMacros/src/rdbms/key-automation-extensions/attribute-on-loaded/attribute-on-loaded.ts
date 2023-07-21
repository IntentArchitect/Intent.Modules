/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    if (!element.hasMetadata(metadataKey.association) ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    const associationTarget = element.getStereotype(foreignKeyStereotypeId)?.getProperty(foreignKeyStereotypeAssociationProperty)?.getValue();
    if (associationTarget == null) {
        if (!element.hasStereotype(foreignKeyStereotypeId)) {
            element.addStereotype(foreignKeyStereotypeId);
        }

        const associationId = element.getMetadata(metadataKey.association);
        element
            .getStereotype(foreignKeyStereotypeId)
            .getProperty(foreignKeyStereotypeAssociationProperty)
            .setValue(associationId);
    }

    if (!element.hasMetadata(metadataKey.fkOriginalName)) {
        element.setMetadata(metadataKey.fkOriginalName, element.getName());
    }
}

execute();