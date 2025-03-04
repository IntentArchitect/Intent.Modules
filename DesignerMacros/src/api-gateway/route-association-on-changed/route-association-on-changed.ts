/// <reference path="../_common/getMethodAndRoute.ts" />
/// <reference path="../_common/updateApiGatewayRouteName.ts" />
/// <reference path="../_common/syncDownstreamContract.ts" />

function applyRouteAssociationOnChangedBehavior(): void {
    let source = association.getOtherEnd().typeReference.getType();
    let target = association.typeReference.getType();

    if (!source) {
        return;
    }

    updateApiGatewayRouteName(source, target);

    if (!target) {
        return;
    }

    const { verb, route, result } = getMethodAndRoute(source);
    const HttpSettings = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    let httpSettings = source.getStereotype(HttpSettings);

    if (!httpSettings) {
        httpSettings = source.addStereotype(HttpSettings);
    }

    httpSettings.getProperty("Route").setValue(route);
    httpSettings.getProperty("Verb").setValue(verb);

    syncDownstreamContract(source, target);
}

applyRouteAssociationOnChangedBehavior();