/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/getParent.ts" />

/**
 * Gets the ultimate target entity and it's owning entity (if it has one) of a mapped Command/Query.
 * @param request The Command or Query that has been mapped
 */
function getMappedEntity(
    request: MacroApi.Context.IElementApi
): {
    entity: IElementApi;
    owningEntity: IElementApi;
} {
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
    const mappingTypeIds = [queryEntityMappingTypeId, createEntityMappingTypeId];

    let entity: IElementApi = null;

    // Basic mapping:
    let mappedElement = request.getMapping()?.getElement();
    if (mappedElement != null) {
        let element = mappedElement;
        while (element != null) {
            if (element?.specialization === "Class") {
                entity = element;
                break;
            }

            element = getParent(element, "Class");
        }
    }

    // Advanced mappings:
    if (mappedElement == null) {
        const targetEntities = request.getAssociations()
            .flatMap(association => association.getMappings()
                .filter(mapping => mappingTypeIds.some(y => mapping.mappingTypeId == y))
                .map(mapping => {
                    let element = mapping.getTargetElement();
                    while (element != null) {
                        if (element.specialization === "Class") {
                            return element;
                        }

                        element = getParent(element, "Class");
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

    return {
        entity: entity,
        owningEntity: DomainHelper.getOwningAggregate(entity)
    };
}
