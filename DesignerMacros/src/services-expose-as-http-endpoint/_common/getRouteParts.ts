/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function getRouteParts(request: IElementApi, mappedDetails: IMappedRequestDetails): string[] {
    if (mappedDetails == null) {
        throw new Error("mappedDetails is required");
    }

    const mappedElement = request.getMapping()?.getElement();
    const routeParts = [];

    // Add the owning entity's ids as parts surrounded with curly braces
    if (mappedDetails.owningEntity != null) {
        routeParts.push(...mappedDetails.ownerKeyFields
            .filter(x => x.existingId != null)
            .map(x => {
                const field = request
                    .getChildren("DTO-Field")
                    .find(field => field.id === x.existingId);

                return `{${toCamelCase(field.getName())}}`;
            }))

        // Add a part for name of the owned entity
        routeParts.push(toKebabCase(singularize(mappedDetails.entity.getName())));
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
        const entityName = mappedDetails.entity.getName();

        let routePart = removePrefix(mappedElement.getName(), "Create", "Update", "Delete", "Add", "Remove");
        routePart = removeSuffix(routePart, "Request", "Query", "Command");

        routeParts.push(removePrefix(toKebabCase(routePart), toKebabCase(singularize(entityName)), toKebabCase(entityName), "-"));
    }

    return routeParts;
}
