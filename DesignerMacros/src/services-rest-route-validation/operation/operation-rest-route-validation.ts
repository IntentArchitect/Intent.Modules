/// <reference path="../_common/common-rest-route-validation.ts" />
/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-rest-route-validation/operation/operation-rest-route-validation.ts
 */

// A GET or QUERY Operation is not required to have a return type: it answers 204 No Content, which is what
// brownfield MVC-style actions (e.g. `return View()` or a redirect) and legacy APIs do.
function validateRestRoutesOperation(element: MacroApi.Context.IElementApi): String{
    return validateRestRoutes(element);
}

//Comment / UnComment below when you publish
// @ts-ignore
return validateRestRoutesOperation(element);

