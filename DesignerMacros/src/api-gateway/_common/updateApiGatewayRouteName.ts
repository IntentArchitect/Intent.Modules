/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function updateApiGatewayRouteName(apiGatewayRoute: MacroApi.Context.IElementApi, downstreamService: MacroApi.Context.IElementApi): void {
    if (apiGatewayRoute.specialization !== "Api Gateway Route") {
        throw Error(`Element "${apiGatewayRoute.id}, ${apiGatewayRoute.getName()}" is not of type Api Gateway Route`);
    }

    if (apiGatewayRoute.getAssociations("Route Association").length === 0) {
        apiGatewayRoute.setName(`DetachedGatewayRoute`, true);
    } else if (apiGatewayRoute.getAssociations("Route Association").length > 1) {
        apiGatewayRoute.setName(`AggregateGatewayRoute`, true);
    } else if (downstreamService) {
        apiGatewayRoute.setName(`${downstreamService.getName()}GatewayRoute`, false);
    }
}