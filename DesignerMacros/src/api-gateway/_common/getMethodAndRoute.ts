/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

interface IMethodAndRouteResult {
    verb: string|null;
    route: string|null;
    result: string|null;
}

function getMethodAndRoute(element: MacroApi.Context.IElementApi): IMethodAndRouteResult {
    // Get custom settings
    const customSettings = element.getStereotype("Http Settings");
    const customRoute = customSettings?.getProperty("Route")?.getValue() as string || "";
    const customMethod = customSettings?.getProperty("Verb")?.getValue() as string || "";

    // Get HTTP settings
    const endpoints = element.getAssociations("Route Association")
        .map(x => x.typeReference.getType())
        .filter(x => x);

    if (endpoints.length === 0) {
        return { verb: "", route: "", result: "None" };
    }
    if (endpoints.length > 1) {
        return { verb: "", route: "", result: "Aggregate" };
    }

    const httpSettings = endpoints[0].getStereotype("Http Settings");
    const httpMethod = httpSettings.getProperty("Verb").getValue() as string;
    let httpRoute = httpSettings.getProperty("Route").getValue() as string;

    if (endpoints[0].getParent()?.hasStereotype("Http Service Settings")) {
        let serviceRoute = endpoints[0].getParent().getStereotype("Http Service Settings")?.getProperty("Route")?.getValue() || "";
        const haveSlash = httpRoute && httpRoute.length > 0 ? "/" : "";
        httpRoute = `${serviceRoute}${haveSlash}${httpRoute}`;
    }

    // Use custom values if available, otherwise fall back to HTTP settings
    return {
        verb: customMethod || httpMethod,
        route: customRoute || httpRoute,
        result: null
    };
}
