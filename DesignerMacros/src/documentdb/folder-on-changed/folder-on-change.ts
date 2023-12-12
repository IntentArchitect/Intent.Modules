/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/folder-on-change/folder-on-change.ts
 */

/// <reference path="../common/updatePartitionKeyAsPrimary.ts" />

const package = element.getPackage();

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!package.hasStereotype(documentStoreId)) {
        return;
    }
    updatePartitionKeyAsPrimary(element);
}

execute();
