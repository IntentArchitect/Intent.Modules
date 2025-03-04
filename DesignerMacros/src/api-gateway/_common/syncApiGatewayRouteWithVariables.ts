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

    const downstreamOperation = downstreamAssociationEnd.typeReference.getType();
    const customRoute = getHttpRouteFromOperation(apiGatewayRoute);
    const httpRoute = getHttpRouteFromOperation(downstreamOperation);

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
        
        let downstreamField = downstreamOperation.getChildren()
            .filter(x => isParamOrFieldFromRoute(downstreamOperation, x) && extractRouteVariables(getHttpRouteFromOperation(downstreamOperation).toLowerCase()).indexOf(routeVar) >= 0)[0];

        if (downstreamField) {
            const routeMappingId = "48776d2f-ee28-4738-844f-d2ee610b516f";
            let mapping = downstreamAssociationEnd.getMapping(routeMappingId);
            if (!mapping) {
                mapping = downstreamAssociationEnd.createAdvancedMapping(apiGatewayRoute.id, downstreamOperation.id, routeMappingId);
            }
            mapping.addMappedEnd("Data Mapping", [apiGatewayRoute.id, field.id], [downstreamOperation.id, downstreamField.id]);
        }
    });
}