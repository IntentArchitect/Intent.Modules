/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/class-on-changed/class-on-changed.ts
 */
/// <reference path="../common/updatePrimaryKey.ts" />
/// <reference path="../common/updateForeignKeysForElement.ts" />

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!element.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    updatePrimaryKey(element);
    updateForeignKeysForElement(element);
}

execute();
