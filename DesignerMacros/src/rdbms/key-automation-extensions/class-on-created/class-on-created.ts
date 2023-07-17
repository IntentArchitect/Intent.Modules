/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.RDBMS
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/rdbms/key-automation-extensions/class-on-created/class-on-created.ts
 */
/// <reference path="../_common/updatePrimaryKeys.ts" />

function execute(): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    updatePrimaryKeys(element);
}

execute();
