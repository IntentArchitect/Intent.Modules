/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    if (element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    const associationTarget = element.getStereotype(foreignKeyStereotypeId)?.getProperty(foreignKeyStereotypeAssociationProperty)?.getValue() as MacroApi.Context.IAssociationApi;
    associationTarget.isSourceEnd();
    associationTarget.getOtherEnd().id

    const existingAssociation = element.getMetadata(metadataKey.association);

    if (associationTarget == null && existingAssociation != null) {
        if (!element.hasStereotype(foreignKeyStereotypeId)) {
            element.addStereotype(foreignKeyStereotypeId);
        }

        const stereotype = element.getStereotype(foreignKeyStereotypeId);
        stereotype.getProperty(foreignKeyStereotypeAssociationProperty).setValue(existingAssociation);
    }

    if (existingAssociation && !element.getMetadata("fk-original-name")) {
        element.setMetadata("fk-original-name", element.getName());
    }
}

execute();