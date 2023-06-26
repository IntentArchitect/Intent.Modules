/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../common/isAggregateRelationship.ts" />

function requiresForeignKey(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return associationEnd.typeReference.isNavigable && 
        isAggregateRelationship(associationEnd);
}
