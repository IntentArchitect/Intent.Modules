/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="./extractRouteVariables.ts" />
/// <reference path="./isParamOrFieldFromRoute.ts" />

function getHttpRouteFromOperation(operation: MacroApi.Context.IElementApi): string {
    const httpSettings = operation?.getStereotype("Http Settings");
    const route = httpSettings?.getProperty("Route")?.getValue() as string || "";
    return route;
}

function syncApiGatewayRouteWithVariables(
    apiGatewayRoute: MacroApi.Context.IElementApi, 
    downstreamAssociationEnd: MacroApi.Context.IAssociationApi): void {

    if (apiGatewayRoute.specialization !== "Api Gateway Route") {
        throw Error(`Element "${apiGatewayRoute.id}, ${apiGatewayRoute.getName()}" is not of type Api Gateway Route`);
    }

    const downstreamOperation = downstreamAssociationEnd?.typeReference.getType();
    const customRoute = getHttpRouteFromOperation(apiGatewayRoute);
    const httpRoute = downstreamOperation ? getHttpRouteFromOperation(downstreamOperation) : null;

    let upstreamRoute = customRoute || httpRoute;
    
    if (!upstreamRoute) {
        apiGatewayRoute.getChildren("DTO-Field").forEach(x => x.delete());
        return;
    }

    let routeVars = extractRouteVariables(upstreamRoute);
    apiGatewayRoute.getChildren("DTO-Field")
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

        // Go back to commit [755e8b97d879661be312ae7cb0e22cd0c5bf1d2c] when you need the Mappings
    });
}