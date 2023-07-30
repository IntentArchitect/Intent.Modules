/**
 * Workaround for element's from referenced packages not having getParent()
 * @param element The element whose parent should be searched for
 * @param parentSpecializationType The specialization type of the parent
 */
function getParent(element: MacroApi.Context.IElementApi, parentSpecializationType: string): MacroApi.Context.IElementApi {
    const elements = lookupTypesOf(parentSpecializationType);

    const parent = elements
        .find(x => x.getChildren(element.specialization)
            .some(child => child.id === element.id));

    if (parent == null) {
        throw new Error("Could not find parent");
    }

    return parent;
}