/// <reference path="../_common/extractRouteVariables.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function applyOnChangedApiGatewayRouteBehavior(): void {
    const customSettings = element?.getStereotype("Http Settings");
    const customRoute = customSettings?.getProperty("Route")?.getValue() as string || "";

    const endpoints = element.getAssociations("Route Association")
        .map(x => x.typeReference.getType())
        .filter(x => x);

    const httpSettings = endpoints[0]?.getStereotype("Http Settings");
    const httpRoute = httpSettings?.getProperty("Route")?.getValue() as string;

    let upstreamRoute = customRoute || httpRoute;

    if (!upstreamRoute) {
        element.getChildren("DTO-Field").filter(field => field.getName() !== "Body").forEach(x => x.delete());
        return;
    }

    let routeVars = extractRouteVariables(upstreamRoute);
    element.getChildren("DTO-Field").filter(field => field.getName() !== "Body")
        .forEach(field => {
            let routeVar = field.getMetadata("routeVar");
            if (routeVars.indexOf(routeVar) < 0) {
                field.delete();
            }
        });
    routeVars.forEach(routeVar => {
        let field = element.getChildren("DTO-Field").filter(x => x.getMetadata("routeVar") === routeVar)[0] 
            ?? createElement("DTO-Field", toPascalCase(routeVar), element.id);
        field.setMetadata("routeVar", routeVar);
    });
}

applyOnChangedApiGatewayRouteBehavior();