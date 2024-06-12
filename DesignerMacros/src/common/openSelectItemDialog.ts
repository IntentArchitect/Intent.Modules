/// <reference path="../../typings/elementmacro.context.api.d.ts" />

interface IISelectElementDialogOptions<TItem> {
    items: TItem[];
    getId(item: TItem): string;
    getDisplayName(item: TItem): string;
    getNoItemsFoundMessage(): string;
    getItemNotFoundMessage(itemId: string): string;
}

function getPackageSelectItemOptions(packages: MacroApi.Context.IPackageApi[], packageTypeName: string): IISelectElementDialogOptions<MacroApi.Context.IPackageApi> {
    return {
        items: packages,
        getId(item) {
            return item.id;
        },
        getDisplayName(item) {
            return item.getName()
        },
        getItemNotFoundMessage(itemId) {
            return `No ${packageTypeName} found with id "${itemId}".`;
        },
        getNoItemsFoundMessage() {
            return `No packages of type ${packageTypeName} could be found.`;
        },
    };
}

function getElementSelectItemOptions(elements: MacroApi.Context.IElementApi[], elementTypeName: string, relevantPackageTypeName: string): IISelectElementDialogOptions<MacroApi.Context.IElementApi> {
    return {
        items: elements,
        getId(item) {
            return item.id;
        },
        getDisplayName(item) {
            return item.getName()
        },
        getItemNotFoundMessage(itemId) {
            return `No "${elementTypeName}" found with id "${itemId}".`;
        },
        getNoItemsFoundMessage() {
            return `No Elements of type "${elementTypeName}" could be found. Please ensure that you have a reference to the ${relevantPackageTypeName} package and that at least one ${elementTypeName} exists in it.`;
        },
    };
}

/**
 * Dialog selection.
 * @param options For simplicity, use getPackageSelectItemOptions() or getElementSelectItemOptions()
 * @returns Selected item.
 */
async function openSelectItemDialog<TItem>(options: IISelectElementDialogOptions<TItem>): Promise<TItem> {
    if (!options) {
        throw new Error("Options are required for 'openSelectItemDialog'.");
    }
    
    let items = options.items;
    if (items.length == 0) {
        await dialogService.info(options.getNoItemsFoundMessage());
        return null;
    }

    let itemId = await dialogService.lookupFromOptions(items.map(item => ({
        id: options.getId(item),
        name: options.getDisplayName(item)
    })));

    if (itemId == null) {
        await dialogService.error(options.getItemNotFoundMessage(itemId));
        return null;
    }

    let foundItem = items.filter(item => options.getId(item) === itemId)[0];
    return foundItem;
}