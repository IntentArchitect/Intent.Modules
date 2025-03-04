/// <reference path="../_common/getMethodAndRoute.ts" />

function formatMethodAndRoute(element: MacroApi.Context.IElementApi): string {
    const { verb, route, result } = getMethodAndRoute(element);
    
    if (result === "None") return "";
    if (result === "Aggregate") return "";
    
    return `${verb}`;
}

// Uncomment the following:
//return formatMethodAndRoute(element);