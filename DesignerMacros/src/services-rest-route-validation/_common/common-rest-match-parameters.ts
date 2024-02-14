/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function matchParameters(element: MacroApi.Context.IElementApi, routeToCheck: string): string | null {
    // Extract all parameters from the route
    let routeParameters = routeToCheck.match(/{([^}]*)}/g) || [];
    let elementType = element.specialization; // Assuming this is how you identify the element type
    let elementChildren;

    if (elementType === "Command" || elementType === "Query") {
        // For Commands and Queries, consider children that are properties
        elementChildren = element.getChildren("Property");
    } else if (elementType === "Operation") {
        // For Operations, consider children that are parameters
        elementChildren = element.getChildren("Parameter");
    } else {
        return null; // Return no message if element type is unrecognized
    }

    // Normalize route parameters by trimming braces
    let normalizedRouteParameters = routeParameters.map(param => param.replace(/[{}]/g, ""));

    // Collect all names of properties/parameters for the element
    let elementNames = elementChildren.map(child => child.getName());

    // Find any route parameters not present in element properties/parameters
    let unmatchedParameters = normalizedRouteParameters.filter(param => !elementNames.includes(param));

    if (unmatchedParameters.length > 0) {
        // If there are unmatched parameters, return a validation message
        return "Route mismatch: some route parameters do not match element's properties/parameters.";
    }

    return null; // Return no message if all validations pass
}