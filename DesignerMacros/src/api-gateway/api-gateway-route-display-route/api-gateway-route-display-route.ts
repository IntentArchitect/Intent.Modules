/// <reference path="../_common/getMethodAndRoute.ts" />

function formatMethodAndRoute(element: MacroApi.Context.IElementApi): string {
    const { verb, route, result } = getMethodAndRoute(element);
    
    if (result === "None") return "[Disconnected]";
    if (result === "Aggregate") return "[Aggregate]";
    
    return `${route}`;
}

// Uncomment the following:
//return formatMethodAndRoute(element);