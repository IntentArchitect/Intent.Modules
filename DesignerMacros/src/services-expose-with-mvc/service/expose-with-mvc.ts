/// <reference path="../operation/shared.ts" />

function exposeWithMvc(service: MacroApi.Context.IElementApi): void {
    const findAllOperation = getFindAllOperation(service);

    for (const operation of service.getChildren("Operation")) {
        applyToOperation(operation, findAllOperation);
    }
}

/**
 * Used by Intent.Modules.NET\Modules\Intent.AspNetCore.MVC
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-expose-with-mvc/service/expose-with-mvc.ts
 */
exposeWithMvc(element);