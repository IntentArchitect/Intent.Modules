/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function getRouteParts(request: IElementApi, domainElement: MappedDomainElement): string[] {
    if (domainElement == null) {
        throw new Error("entity is required");
    }

    // all the default CQRS endpoints
    let defaultCQRSEndpoints = [`Create${domainElement.getName()}Command`, `Delete${domainElement.getName()}Command`,
        `Update${domainElement.getName()}Command`, `Get${pluralize(domainElement.getName())}Query`,
        `Get${domainElement.getName()}ByIdQuery`
    ];

    let isDefaultEndpoint = defaultCQRSEndpoints.includes(request.getName());

    // Following the RESTful naming conventions from https://restfulapi.net/resource-naming/.
    // RESTful naming conventions applied, with the exception of Operation names.
    // Operation names are developer-defined and should be left as-is to avoid potential unintended side effects.

    let routeParts: string[] = [];
    const mappedDetails = getMappedRequestDetails(request);

    if (!mappedDetails && isDefaultEndpoint) {
        return routeParts;
    }else if(!mappedDetails && !isDefaultEndpoint) {
        routeParts.push(...generateNonDefaultEndpointRouteName(request, domainElement.getName()));
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

    if (!isDefaultEndpoint && mappedDetails.mappingTargetType !== "Operation") {
        routeParts.push(...generateNonDefaultEndpointRouteName(request, domainElement.getName()));
    }

    return routeParts;
}

function generateNonDefaultEndpointRouteName(operation: IElementApi, domainName: string, additionalReplacement: string[] = []): string[] {
    let operationName = operation.getName();
    let routeParts: string[] = [];

    // filter out some common phrases
    let toReplace = [
        `Get${pluralize(domainName)}`, `Find${pluralize(domainName)}`, `Lookup${pluralize(domainName)}`, 
        `Get${domainName}`, `Find${domainName}`, `Lookup${domainName}`,
        `${domainName}`, `${pluralize(domainName)}`,
        `Query`, `Request`, `ById`, `Create`, `Update`, `Delete`, `Modify`, `Insert`, `Patch`, `Remove`, `Add`, `Set`, `List`, `Command`
    ];

    // additionalReplacement will mostly contain the folder paths. So if in the Service designer there is a Query/Command in a folder, 
    // we would want to replace that in the generated route. E.g. A query called "GetSpecialProductDataQuery", NOT linked to a domain
    // put in the product folder, instead of it generating "special-product-data", it should generate "special-data".
    // additionalReplacement will contain "product", so we supplement it with "Product", "Products" and "products"
    let supplementAdditionalReplacement: string[] = [];
    additionalReplacement.forEach((replacement) => {
        supplementAdditionalReplacement.push(pluralize(replacement[0].toUpperCase() + replacement.substring(1)));
        supplementAdditionalReplacement.push(pluralize(replacement));
        supplementAdditionalReplacement.push(replacement[0].toUpperCase() + replacement.substring(1));
        supplementAdditionalReplacement.push(singularize(replacement[0].toUpperCase() + replacement.substring(1)));
        supplementAdditionalReplacement.push(singularize(replacement));
    });

    toReplace.push(...supplementAdditionalReplacement);
    toReplace.push(...additionalReplacement);
    
    // sort longest to shortest
    toReplace.sort((a, b) => b.length - a.length).forEach((search) => {
        operationName = operationName.replace(search, '');
    });
    
    // convert to kebab case, and then correct it based on acronyms (e.g. SMS)
    let cleanedOperationName = kebabCaseAcronymCorrection(toKebabCase(operationName), operationName);

    if (cleanedOperationName) {
        routeParts.push(cleanedOperationName);
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

// this method will correct incorrect acronym/initialism in the kebab case.
// For example, "SendSMS" will become "send-s-m-s", and this method will correct it to "send-sms"
function kebabCaseAcronymCorrection(kebabInput: string, originalInput: string): string {
    
     // Split the kebab-case result into individual parts
     let parts = kebabInput.split('-');
     let correctedParts: string[] = [];
     var currentPosition = originalInput;
     var currentWord = "";

     for(let part of parts) {
        if (part.length != 1) {
            if(currentWord != ""){
                correctedParts.push(currentWord);
                currentWord = "";
            }

            correctedParts.push(part);
            currentPosition = currentPosition.substring(part.length);
            continue;
        }

        if(currentPosition.startsWith(part.toUpperCase())) {
            currentWord = currentWord + part;
            currentPosition = currentPosition.substring(part.length);
        }
     }

     if(currentWord != ""){
        correctedParts.push(currentWord);
        currentWord = "";
    }
 
     // Join the corrected parts back into a kebab-case string
     return correctedParts.join('-');
 }

