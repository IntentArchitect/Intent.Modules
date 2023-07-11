/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />
type IElementApi = MacroApi.Context.IElementApi;

function execute(): void {
    if (element.hasMetadata(isBeingDeletedByScript) ||
        !element.hasStereotype(primaryKeyStereotypeId)
    ) {
        return;
    }

    element.getParent().setMetadata(autoManageKeys, "false");
}

execute();