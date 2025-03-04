/// <reference path="../_common/syncApiGatewayRouteWithVariables.ts" />
/// <reference path="../_common/syncDownstreamContract.ts" />


function applyOnChangedApiGatewayRouteBehavior(): void {
    let targets = element.getAssociations("Route Association");
    for (let targetEnd of targets.filter(x => x.typeReference?.getType())) {
        syncDownstreamContract(element, targetEnd.typeReference.getType());
    }
    syncApiGatewayRouteWithVariables(element);
}

applyOnChangedApiGatewayRouteBehavior();