/// <reference path="../common/operation-validation.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Modelers.Services
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-operation-validation/operation-parameter/operation-validation.ts
 */

function validateOperationParameter(element: MacroApi.Context.IElementApi): String{   
    let operation = element.getParent();
    return validateDuplicateOperation(operation);
}


//Comment / UnComment below when you publish
validateOperationParameter(element);
//return validateOperationParameter(lookup(id));