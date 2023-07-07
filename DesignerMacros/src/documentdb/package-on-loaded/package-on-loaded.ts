/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/package-on-loaded/package-on-loaded.ts
 */

/// <reference path="../../common/ensureIsAtTargetVersion.ts" />
/// <reference path="../common/updatePrimaryKey.ts" />
/// <reference path="../common/updateForeignKeys.ts" />

const package = element.getPackage();

ensureIsAtTargetVersion(package, "document-db-on-package-load-applied-version", 1, () => {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    package.getStereotypes()
    if (!package.hasMetadata("database-paradigm-selected") && 
        !package.hasStereotype(documentStoreId) && 
        !package.hasStereotype("Relational Database")
    ) {
        package.addStereotype(documentStoreId);
    }

    package.setMetadata("database-paradigm-selected", "true");

    if (!package.hasStereotype(documentStoreId)) {
        return;
    }

    let classes = lookupTypesOf("Class").filter(x => x.getPackage().id === package.id);
    for (let classElement of classes) {
        updatePrimaryKey(classElement);
        updateForeignKeysForElement(classElement);
    }
});
