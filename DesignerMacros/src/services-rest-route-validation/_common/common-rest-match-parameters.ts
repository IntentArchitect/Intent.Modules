/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function matchParameters(element: MacroApi.Context.IElementApi, routeToCheck: string): string | null {
    let httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    let apiVersionSettingId = "20855f03-c663-4ec6-b106-de06be98f1fe"; // Api Version Setting

    let elementHasApiVersion = element.getStereotype(apiVersionSettingId) !== null;

    if (elementHasApiVersion) {
        // Assuming routeToCheck is a string representing your route
        routeToCheck = routeToCheck.replace(/{version}/g, '');
    }

    // Extract all parameters from the route
    let routeParameters = routeToCheck.match(/{([^}]*)}/g) || [];
    let elementType = element.specialization; // Assuming this is how you identify the element type
    let elementChildren;

    if (elementType === "Command" || elementType === "Query") {
        // For Commands and Queries, consider children that are properties
        elementChildren = element.getChildren("DTO-Field");
    } else if (elementType === "Operation") {
        // For Operations, consider children that are parameters
        elementChildren = element.getChildren("Parameter");
    } else {
        return null; // Return no message if element type is unrecognized
    }

    // Normalize route parameters by trimming braces and converting to lower case for case-insensitive comparison
    let normalizedRouteParameters = routeParameters.map(param => param.replace(/[{}\*]/g, "").toLowerCase());

    console.log(`Normalized route parameters found: ${normalizedRouteParameters.join(", ")}`);

    // Collect all names of properties/parameters for the element and convert them to lower case
    let elementNames = elementChildren.map(child => child.getName().replace(/[\*]/g, "").toLowerCase());

    console.log(`Element properties/parameters names found: ${elementNames.join(", ")}`);

    // Ensure the comparison against element names is case-insensitive
    let unmatchedParameters = normalizedRouteParameters.filter(param => !elementNames.includes(param.toLowerCase()) && param.toLowerCase() !== "version");

    if (unmatchedParameters.length > 0) {
        console.log(`Unmatched parameters found: ${unmatchedParameters.join(", ")}`);
        // Concatenate the unmatched parameters to the error message
        return `Route mismatch: some route parameters do not match element's properties/parameters. Unmatched parameters: ${unmatchedParameters.join(", ")}`;
    }

    console.log("No unmatched parameters found. Validation passed.");
    return null; // Return no message if all validations pass
}
