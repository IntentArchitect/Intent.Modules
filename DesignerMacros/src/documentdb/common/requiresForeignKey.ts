/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../common/isAggregateRelationship.ts" />

function requiresForeignKey(associationEnd: MacroApi.Context.IAssociationApi): boolean {
    // Check if the association is a self-reference
    if (associationEnd.typeReference.typeId === associationEnd.getOtherEnd().typeReference.typeId) {
        return true;
    }

    return associationEnd.typeReference.isNavigable &&
        isAggregateRelationship(associationEnd);
}
