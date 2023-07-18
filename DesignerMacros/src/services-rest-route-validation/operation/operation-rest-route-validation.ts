/// <reference path="../common/common-rest-route-validation.ts" />
/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-rest-route-validation/operation/operation-rest-route-validation.ts
 */

function validateRestRoutesOperation(element: MacroApi.Context.IElementApi): String{   
    if (element.typeReference.typeId == null && element.getStereotype("Http Settings")?.getProperty("Verb").value === "GET") {
        return "Return Type required for HTTP GET Verb";
    }
    return validateRestRoutes(element);
}

validateRestRoutesOperation(element);
//return validateRestRoutesOperation(lookup(id));

