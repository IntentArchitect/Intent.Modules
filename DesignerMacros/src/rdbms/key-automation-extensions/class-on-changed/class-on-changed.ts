/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.RDBMS
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/rdbms/key-automation-extensions/class-on-changed/class-on-changed.ts
 */
/// <reference path="../_common/updatePrimaryKeys.ts" />
/// <reference path="../_common/updateForeignKeys.ts" />

function execute(): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit") {
        return;
    }

    updatePrimaryKeys(element);
    updateForeignKeysAndForDerived(element);

    function updateForeignKeysAndForDerived(forElement: IElementApi): void {
        if (forElement.getMetadata(autoManageKeys) !== "false") {
            for (const association of forElement.getAssociations("Association")) {
                updateForeignKeys(association);
                updateForeignKeys(association.getOtherEnd());
            }

            var associationIds = forElement.getAssociations("Association")
                .map(x => x.id);
            var fkAttributesToDelete = forElement.getChildren("Attribute")
                .filter(association =>
                    association.hasStereotype(foreignKeyStereotypeId) &&
                    association.hasMetadata("association") &&
                    !associationIds.some(id => id === association.getMetadata("association")))
            for (const fkAttribute of fkAttributesToDelete) {
                fkAttribute.setMetadata(isBeingDeletedByScript, "true");
                fkAttribute.delete();
            }
        }

        var derivedTypes = forElement.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());

        for (const derivedType of derivedTypes) {
            updateForeignKeysAndForDerived(derivedType);
        }
    }
}

execute();
