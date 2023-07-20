/// <reference path="../common/operation-validation.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Modelers.Services
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-operation-validation/operation/operation-validation.ts
 */

function validateOperation(element: MacroApi.Context.IElementApi): String{   
    return validateDuplicateOperation(element);
}

//Comment / UnComment below when you publish
validateOperation(element);
//return validateOperation(lookup(id));