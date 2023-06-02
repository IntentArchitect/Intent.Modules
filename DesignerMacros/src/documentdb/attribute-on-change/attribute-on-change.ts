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
    const PrimaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    const ForeignKeyStereotypeId = "ced3e970-e900-4f99-bd04-b993228fe17d";

    if (element.getMetadata("is-managed-key") != "true") {
        if (element.hasStereotype(PrimaryKeyStereotypeId)) {
            element.removeStereotype(PrimaryKeyStereotypeId);
        }
        if (element.hasStereotype(ForeignKeyStereotypeId)) {
            element.removeStereotype(ForeignKeyStereotypeId);
        }
        
        return;
    }

    if (!element.hasMetadata("association") && !element.hasStereotype(PrimaryKeyStereotypeId)) {
        element.addStereotype(PrimaryKeyStereotypeId);
    }

    if (element.hasMetadata("association") && !element.hasStereotype(ForeignKeyStereotypeId)) {
        element.addStereotype(ForeignKeyStereotypeId);
    }
}

execute();