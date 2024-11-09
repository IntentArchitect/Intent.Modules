/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/association-on-changed/association-on-changed.ts
 */

/// <reference path="../common/updatePrimaryKey.ts" />
/// <reference path="../common/updateForeignKeysForAssociation.ts" />

function execute() {
    // The user is busy connecting the association and hasn't attached it to an element yet.
    if (association.typeReference.getType() == null) {
        return;
    }

    let associationEnd = association.getOtherEnd().typeReference.getType();

    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!associationEnd.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    updatePrimaryKey(association.typeReference.getType());
    updateForeignKeysForAssociation(association);
}

execute();