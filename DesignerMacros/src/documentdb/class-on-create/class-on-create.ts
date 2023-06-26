/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/class-on-create/class-on-create.ts
 */
/// <reference path="../common/getDefaultIdType.ts" />

function execute() {
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!element.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    const primaryKeyStereotypeId: string = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";

    let idAttr = createElement("Attribute", "Id", element.id);
    idAttr.typeReference.setType(getDefaultIdType());
    idAttr.addMetadata("is-managed-key", "true");
    idAttr.addStereotype(primaryKeyStereotypeId);
}

execute();