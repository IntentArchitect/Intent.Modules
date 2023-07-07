/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/attribute-on-deleted/attribute-on-deleted.ts
 */
/// <reference path="../common/isAggregateRoot.ts" />
/// <reference path="../common/updateForeignKeys.ts" />

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!element.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    if (element.getMetadata("is-managed-key") != "true" || !isAggregateRoot(element.getParent())) {
        return;
    }

    if (!element.hasMetadata("association")) {
        const PrimaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
        let idAttr = createElement("Attribute", "Id", element.getParent().id);
        idAttr.typeReference.setType(element.typeReference.getType().id);
        idAttr.setOrder(0);
        idAttr.addMetadata("is-managed-key", "true");
        idAttr.addStereotype(PrimaryKeyStereotypeId);
        return;
    }

    updateForeignKeysForElement(element.getParent());
}

execute();