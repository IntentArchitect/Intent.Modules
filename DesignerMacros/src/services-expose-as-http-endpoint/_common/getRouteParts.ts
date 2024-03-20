/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function getRouteParts(request: IElementApi, domainElement: MappedDomainElement): string[] {
    if (domainElement == null) {
        throw new Error("entity is required");
    }

    const routeParts: string[] = [];

    // Basic mapping:
    const mappedElement = request.getMapping()?.getElement();
    if (mappedElement != null) {
        const mappedDetails = getMappedRequestDetails(request);

        // Add the owning entity's ids as parts surrounded with curly braces
        if (domainElement.entityDomainElementDetails?.hasOwningEntity() == true) {
            routeParts.push(...mappedDetails.ownerKeyFields
                .filter(x => x.existingId != null)
                .map(x => {
                    const field = request
                        .getChildren("DTO-Field")
                        .find(field => field.id === x.existingId);

                    return `{${toCamelCase(field.getName())}}`;
                }))

            // Add a part for name of the owned entity
            routeParts.push(toKebabCase(singularize(domainElement.getName())));
        }

        // Add the entity's ids as parts surrounded with curly braces
        routeParts.push(...mappedDetails.entityKeyFields
            .filter(x => x.existingId != null)
            .map(x => {
                const field = request
                    .getChildren("DTO-Field")
                    .find(field => field.id === x.existingId);

                return `{${toCamelCase(field.getName())}}`;
            }))

        // Add the operation's name:
        if (mappedDetails.mappingTargetType === "Operation") {
            const entityName = domainElement.getName();

            let routePart = removePrefix(mappedElement.getName(), "Create", "Update", "Delete", "Add", "Remove");
            routePart = removeSuffix(routePart, "Request", "Query", "Command");

            routeParts.push(removePrefix(toKebabCase(routePart), toKebabCase(singularize(entityName)), toKebabCase(entityName), "-"));
        }

        return routeParts;
    }

    // Advanced mapping:
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
    const updateEntityMappingTypeId = "01721b1a-a85d-4320-a5cd-8bd39247196a";
    const mappingTypeIds = [queryEntityMappingTypeId, createEntityMappingTypeId, updateEntityMappingTypeId];
    const associationWithMapping = request
        .getAssociations()
        .find(association => association.hasMappings() && association
            .getMappings()
            .every(mapping => {
                if (!mappingTypeIds.includes(mapping.mappingTypeId)) {
                    return false;
                }

                let element = mapping.getTargetElement();
                while (element != null) {
                    if (element.specialization === "Class") {
                        break;
                    }

                    element = getParent(element, "Class");
                }

                if (element == null) {
                    return false;
                }

                return element.id === domainElement.getId();
            }));

    if (associationWithMapping != null) {
        const mappings = associationWithMapping.getMappings();
        const queryMapping = mappings.find(x => x.mappingTypeId === queryEntityMappingTypeId);
        const createMapping = mappings.find(x => x.mappingTypeId === createEntityMappingTypeId);
        const updateMapping = mappings.find(x => x.mappingTypeId === updateEntityMappingTypeId);

        // Add the owning entity's ids as parts surrounded with curly braces
        if (domainElement.entityDomainElementDetails?.hasOwningEntity() == true) {
            if (queryMapping != null) {
                let applicableClassIds = getEntityInheritanceHierarchyIds(domainElement?.entityDomainElementDetails?.owningEntity);
                routeParts.push(...queryMapping.getMappedEnds()
                    .filter(end => applicableClassIds.some(x => x === getParent(end.getTargetElement(), "Class").id))
                    .map(x => `{${toCamelCase(x.getSourceElement().getName())}}`));
            }

            // Add a part for name of the owned entity
            routeParts.push(toKebabCase(singularize(domainElement.getName())));
        }

        // Add the entity's ids as parts surrounded with curly braces
        if (queryMapping != null) {
            let applicableClassIds = getEntityInheritanceHierarchyIds(domainElement?.entityDomainElementDetails?.entity);
            routeParts.push(...queryMapping.getMappedEnds()
                .filter(end => applicableClassIds.some(x => x === getParent(end.getTargetElement(), "Class").id))
                .map(x => `{${toCamelCase(x.getSourceElement().getName())}}`));
        }
        
        // Add the operation's name:
        const mapping = createMapping ?? updateMapping;
        if (mapping == null) {
            return routeParts;
        }

        var mappingEnd = mapping.getMappedEnds().find(x => x.getSourceElement().id === request.id);
        if (mappingEnd == null) {
            return routeParts;
        }

        const mappedElement = mappingEnd.getTargetElement();
        if (mappedElement?.specialization === "Operation") {
            const entityName = domainElement.getName();

            let routePart = removePrefix(mappedElement.getName(), "Create", "Update", "Delete", "Add", "Remove");
            routePart = removeSuffix(routePart, "Request", "Query", "Command");

            routeParts.push(removePrefix(toKebabCase(routePart), toKebabCase(singularize(entityName)), toKebabCase(entityName), "-"));
        }

        return routeParts;
    }

    return routeParts;

    function getEntityInheritanceHierarchyIds(curEntity: IElementApi) : string[] {
        let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
        if (generalizations.length == 0) {
            return [curEntity.id];
        }
        let other = getEntityInheritanceHierarchyIds(generalizations[0].typeReference.getType());
        return other.concat(curEntity.id);
    }
}
