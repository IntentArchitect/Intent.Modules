/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/attribute-on-change/attribute-on-change.ts
 */

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!element.getPackage().hasStereotype(documentStoreId)) {
        return;
    }
    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    const foreignKeyStereotypeId = "ced3e970-e900-4f99-bd04-b993228fe17d";

    if (element.getMetadata("is-managed-key") != "true") {
        if (element.hasStereotype(primaryKeyStereotypeId)) {
            element.removeStereotype(primaryKeyStereotypeId);
        }
        if (element.hasStereotype(foreignKeyStereotypeId)) {
            element.removeStereotype(foreignKeyStereotypeId);
        }
        
        return;
    }

    if (!element.hasMetadata("association") && !element.hasStereotype(primaryKeyStereotypeId)) {
        element.addStereotype(primaryKeyStereotypeId);
    }

    if (element.hasMetadata("association") && !element.hasStereotype(foreignKeyStereotypeId)) {
        element.addStereotype(foreignKeyStereotypeId);
    }
}

execute();