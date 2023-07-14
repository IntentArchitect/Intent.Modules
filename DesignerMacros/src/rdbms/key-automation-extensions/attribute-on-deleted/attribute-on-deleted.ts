/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />
/// <reference path="../_common/updateForeignKeys.ts" />
type IElementApi = MacroApi.Context.IElementApi;

function execute(): void {
    if (element.hasMetadata(isBeingDeletedByScript) ||
        !element.hasMetadata(isManagedKey)
    ) {
        return;
    }

    const classElement = element.getParent();

    classElement.setMetadata(autoManageKeys, "false");

    for (const association of classElement.getAssociations("Association")) {
        updateForeignKeys(association.getOtherEnd());
    }
}

execute();