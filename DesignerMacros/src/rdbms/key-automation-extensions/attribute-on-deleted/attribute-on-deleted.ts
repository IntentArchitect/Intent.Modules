/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />
/// <reference path="../_common/updateForeignKeys.ts" />

function execute(): void {
    if (element.hasMetadata(metadataKey.isBeingDeletedByScript) ||
        !element.hasMetadata(metadataKey.isManagedKey) ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    const classElement = element.getParent();

    if (element.hasStereotype(primaryKeyStereotypeId)) {
        classElement.setMetadata(metadataKey.autoManageKeys, "false");
    }

    for (const association of classElement.getAssociations("Association")) {
        updateForeignKeys(association.getOtherEnd());
    }
}

execute();