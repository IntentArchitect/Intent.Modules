/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/association-on-delete/association-on-delete.ts
 */
/// <reference path="../common/updatePrimaryKey.ts" />
/// <reference path="../common/removeAssociatedForeignKeys.ts" />

function execute() {
    let targetClass = association.typeReference.getType();
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!targetClass.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    updatePrimaryKey(targetClass);
    removeAssociatedForeignKeys(association);
}

execute();