/// <reference path="../../typings/elementmacro.context.api.d.ts" />

interface IISelectElementDialogOptions {
    elementType: string;
    packageTypeName: string;
    filterPredicate?(element: MacroApi.Context.IElementApi): boolean;
    getFriendlyElementName?(element: MacroApi.Context.IElementApi): string;
}

async function openSelectElementDialog(options: IISelectElementDialogOptions): Promise<MacroApi.Context.IElementApi> {
    if (!options) {
        throw new Error("Options are required for 'openSelectElementDialog'.");
    }

    const predicate = options.filterPredicate ? options.filterPredicate : function (el: MacroApi.Context.IElementApi) { return true; };
    let elements = lookupTypesOf(options.elementType).filter(x => predicate(x));
    if (elements.length == 0) {
        await dialogService.info(`No Elements of type "${options.elementType}" could be found. Please ensure that you have a reference to the ${options.packageTypeName} package and that at least one ${options.elementType} exists in it.`);
        return null;
    }

    const friendlyName = options.getFriendlyElementName ? options.getFriendlyElementName : function (el: MacroApi.Context.IElementApi) { return el.getName(); };
    let elementId = await dialogService.lookupFromOptions(elements.map((x) => ({
        id: x.id,
        name: friendlyName(x)
    })));

    if (elementId == null) {
        await dialogService.error(`No "${options.elementType}" found with id "${elementId}".`);
        return null;
    }

    let foundElement = lookup(elementId);
    return foundElement;
}