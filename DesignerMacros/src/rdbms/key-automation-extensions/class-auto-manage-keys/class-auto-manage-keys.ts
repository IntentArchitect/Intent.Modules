/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.RDBMS
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/rdbms/key-automation-extensions/class-auto-manage-keys.ts/class-auto-manage-keys.ts
 */
/// <reference path="../_common/updatePrimaryKeys.ts" />

function execute(): void {
    if (!element.getPackage().hasStereotype(relationalDatabaseId)) {
        return;
    }

    element.removeMetadata(autoManageKeys);

    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    updatePrimaryKeys(element);
}

execute();
