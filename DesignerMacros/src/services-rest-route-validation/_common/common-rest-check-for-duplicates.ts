/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function checkForDuplicates(elements: MacroApi.Context.IElementApi[]
    , element: MacroApi.Context.IElementApi
    , routeToCheck: string
    , verbToCheck: string
    , match: (possibleDuplicate: MacroApi.Context.IElementApi, changingElement: MacroApi.Context.IElementApi, routeToCheck: string, verbToCheck: string) => boolean): string | null {

    let duplicate: MacroApi.Context.IElementApi | undefined;
    elements.forEach(e => {
        if (duplicate != null)
            return;
        if (e.id == element.id)
            return;
        if (match(e, element, routeToCheck, verbToCheck)) {
            duplicate = e;
        }
    });
    if (duplicate) {
        return `Duplicate rest route ${element.getName()}(${element.specialization}) with ${duplicate.getName()}(${duplicate.specialization}) - ${routeToCheck}`;
    }
    return null;
}
