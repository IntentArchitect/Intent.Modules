/// <reference path="../_common/common-rest-route-validation.ts" />
/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-rest-route-validation/operation/operation-rest-route-validation.ts
 */

function validateRestRoutesOperation(element: MacroApi.Context.IElementApi): String{
    const verb = element.getStereotype("Http Settings")?.getProperty("Verb").value;
    if (element.typeReference.typeId == null && (verb === "GET" || verb === "QUERY")) {
        return `Return Type required for HTTP ${verb} Verb`;
    }
    return validateRestRoutes(element);
}

//Comment / UnComment below when you publish
// @ts-ignore
return validateRestRoutesOperation(element);

