/// <reference path="mappedDomainElement.ts" />

/**
 * Gets the ultimate target entity and it's owning entity (if it has one) of a mapped Command/Query.
 * @param request The Command or Query that has been mapped
 */
function getMappedDomainElement(request: MacroApi.Context.IElementApi): MappedDomainElement {
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
    const mappingTypeIds = [queryEntityMappingTypeId, createEntityMappingTypeId];
    const mappableElements = ["Class", "Repository"];
    const isMappableElement = function (element: MacroApi.Context.IElementApi): Boolean {
        return mappableElements.some(x => element?.specialization === x);
    };

    let entity: IElementApi = null;

    // Basic mapping:
    let mappedElement = request.getMapping()?.getElement();
    if (mappedElement != null) {
        let element = mappedElement;
        while (element != null) {
            if (isMappableElement(element)) {
                entity = element;
                break;
            }

            element = element.getParent();
        }
    }

    // Advanced mappings:
    if (mappedElement == null) {
        const targetEntities = request.getAssociations()
            .flatMap((association : MacroApi.Context.IBackwardCompatibleIAssociationApi) => association.getMappings()
                .filter(mapping => mappingTypeIds.some(y => mapping.mappingTypeId == y))
                .map(mapping => {
                    let element = mapping.getTargetElement();
                    while (element != null) {
                        if (isMappableElement(element)) {
                            return element;
                        }

                        element = element.getParent();
                    }

                    return null;
                })
                .filter(entity => entity != null));

        // Only if all the targetClasses are the same:
        if (targetEntities.length > 0 && targetEntities.every(x => x.id === targetEntities[0].id)) {
            entity = targetEntities[0];
        }
    }

    if (entity == null) {
        return null;
    }

    return new MappedDomainElement(entity);
}