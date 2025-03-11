/// <reference path="../_common/syncApiGatewayRouteWithVariables.ts" />
/// <reference path="../_common/syncDownstreamContract.ts" />


function applyOnChangedApiGatewayRouteBehavior(): void {
    let targets = element.getAssociations("Route Association") as MacroApi.Context.IAssociationApi[];
    for (let targetEnd of targets.filter(x => x.typeReference?.getType())) {
        syncDownstreamContract(element, targetEnd.typeReference.getType());
        syncApiGatewayRouteWithVariables(element, targetEnd);
    }

    const HttpSettings = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    if (!element.hasStereotype(HttpSettings)) {
        syncApiGatewayRouteWithVariables(element, null);
    }
}

applyOnChangedApiGatewayRouteBehavior();