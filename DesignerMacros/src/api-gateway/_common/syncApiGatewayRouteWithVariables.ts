/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="./extractRouteVariables.ts" />

function syncApiGatewayRouteWithVariables(apiGatewayRoute: MacroApi.Context.IElementApi) {
    if (apiGatewayRoute.specialization !== "Api Gateway Route") {
        throw Error(`Element "${apiGatewayRoute.id}, ${apiGatewayRoute.getName()}" is not of type Api Gateway Route`);
    }

    const customSettings = element?.getStereotype("Http Settings");
    const customRoute = customSettings?.getProperty("Route")?.getValue() as string || "";

    const endpoints = element.getAssociations("Route Association")
        .map(x => x.typeReference.getType())
        .filter(x => x);

    const httpSettings = endpoints[0]?.getStereotype("Http Settings");
    const httpRoute = httpSettings?.getProperty("Route")?.getValue() as string;

    let upstreamRoute = customRoute || httpRoute;
    
    if (!upstreamRoute) {
        apiGatewayRoute.getChildren("DTO-Field").filter(field => field.getName() !== "Body").forEach(x => x.delete());
        return;
    }

    let routeVars = extractRouteVariables(upstreamRoute);
    apiGatewayRoute.getChildren("DTO-Field").filter(field => field.getName() !== "Body")
        .forEach(field => {
            let routeVar = field.getMetadata("routeVar");
            if (routeVars.indexOf(routeVar) < 0) {
                field.delete();
            }
        });
    routeVars.forEach(routeVar => {
        let field = apiGatewayRoute.getChildren("DTO-Field").filter(x => x.getMetadata("routeVar") === routeVar)[0] 
            ?? createElement("DTO-Field", toPascalCase(routeVar), apiGatewayRoute.id);
        field.setMetadata("routeVar", routeVar);
    });
}