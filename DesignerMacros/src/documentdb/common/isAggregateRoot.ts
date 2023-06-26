/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function isAggregateRoot(element: MacroApi.Context.IElementApi) {
    return !element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}
