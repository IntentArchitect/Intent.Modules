/// <reference path="../common/common-paginate.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Application.Dtos.Pagination
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/services-add-paginate/cqrs/add-pagination-cqrs.ts
 */


function addPagination(element: MacroApi.Context.IElementApi): void{   

    addPagingParameters(element, "DTO-Field");    
    changeReturnType(element);
}

addPagination(element);