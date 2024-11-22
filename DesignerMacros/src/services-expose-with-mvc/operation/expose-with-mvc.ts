/// <reference path="shared.ts" />

function exposeWithMvc(operation: MacroApi.Context.IElementApi): void {
    const findAllOperation = getFindAllOperation(operation.getParent());

    applyToOperation(operation, findAllOperation);
}

/**
 * Used by Intent.Modules.NET\Modules\Intent.AspNetCore.MVC
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-with-mvc/operation/expose-with-mvc.ts
 */

exposeWithMvc(element);