/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/package-on-load/package-on-load.ts
 */

/// <reference path="../common/updatePrimaryKey.ts" />
/// <reference path="../common/updateForeignKeys.ts" />

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    element.getStereotypes()
    if (!element.hasMetadata("database-paradigm-selected") && 
        !element.hasStereotype(documentStoreId) && 
        !element.hasStereotype("Relational Database")) {
        
        element.addStereotype(documentStoreId);
    }
    element.setMetadata("database-paradigm-selected", "true");

    if (!element.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    let classes = lookupTypesOf("Class").filter(x => x.getPackage().id === element.id);
    for (let classElement of classes) {
        updatePrimaryKey(classElement);
        updateForeignKeysForElement(classElement);
    }
}

execute();