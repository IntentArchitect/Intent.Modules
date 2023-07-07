/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/association-on-created/association-on-created.ts
 */

function execute() {
    if (!association) { return; }

    let sourceEnd = association.getOtherEnd().typeReference;
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!sourceEnd.getType().getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    sourceEnd.setIsCollection(false);
    sourceEnd.setIsNullable(false);
}
execute();