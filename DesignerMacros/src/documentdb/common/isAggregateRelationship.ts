/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function isAggregateRelationship(associationEnd: MacroApi.Context.IAssociationReadOnlyApi): boolean {
    let sourceAssociationEnd = associationEnd;
    if (associationEnd.isTargetEnd()) {
        sourceAssociationEnd = sourceAssociationEnd.getOtherEnd();
    }

    return sourceAssociationEnd.typeReference.isNullable || sourceAssociationEnd.typeReference.isCollection;
}
