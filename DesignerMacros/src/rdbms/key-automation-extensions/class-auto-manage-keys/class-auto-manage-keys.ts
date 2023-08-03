/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.RDBMS
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/rdbms/key-automation-extensions/class-auto-manage-keys.ts/class-auto-manage-keys.ts
 */
/// <reference path="../_common/updatePrimaryKeys.ts" />
/// <reference path="../_common/updateForeignKeys.ts" />

function execute(): void {
    if (!element.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    element.removeMetadata(metadataKey.autoManageKeys);

    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    updatePrimaryKeys(element);
    for (const association of element.getAssociations("Association")) {
        updateForeignKeys(association);
        updateForeignKeys(association.getOtherEnd());
    }
}

execute();
