/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function getRouteParts(request: IElementApi, domainElement: MappedDomainElement): string[] {
    if (domainElement == null) {
        throw new Error("entity is required");
    }

    // Following the RESTful naming conventions from https://restfulapi.net/resource-naming/.
    // RESTful naming conventions applied, with the exception of Operation names.
    // Operation names are developer-defined and should be left as-is to avoid potential unintended side effects.

    let routeParts: string[] = [];
    const mappedDetails = getMappedRequestDetails(request);

    if (!mappedDetails) {
        return routeParts;
    }

    // Add the owning entity's ids as parts surrounded with curly braces
    if (domainElement.entityDomainElementDetails?.hasOwningEntity() == true) {
        let parentIdRouteParts = [...mappedDetails.ownerKeyFields
            .filter(x => x.existingId != null)
            .map(x => {
                const field = request
                    .getChildren("DTO-Field")
                    .find(field => field.id === x.existingId);

                return `{${toCamelCase(field.getName())}}`;
            })];
        routeParts.push(...parentIdRouteParts);

        // Add a part for name of the owned entity
        let ownedEntName = toKebabCase(pluralize(domainElement.getName()));
        routeParts.push(ownedEntName);
    }

    // Add the entity's ids as parts surrounded with curly braces
    let entIdRouteParts = [...mappedDetails.entityKeyFields
        .filter(x => x.existingId != null)
        .map(x => {
            const field = request
                .getChildren("DTO-Field")
                .find(field => field.id === x.existingId);

            return `{${toCamelCase(field.getName())}}`;
        })];
    routeParts.push(...entIdRouteParts);

    // Add the operation's name:
    if (mappedDetails.mappingTargetType === "Operation") {
        let operationName = mappedDetails.mappingElement.getName();
        routeParts.push(toKebabCase(operationName));
    }

    return routeParts;
}

function getEntityInheritanceHierarchyIds(curEntity: IElementApi): string[] {
    let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
    if (generalizations.length == 0) {
        return [curEntity.id];
    }
    let other = getEntityInheritanceHierarchyIds(generalizations[0].typeReference.getType());
    return other.concat(curEntity.id);
}
