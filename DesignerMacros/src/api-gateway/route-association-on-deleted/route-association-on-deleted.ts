/// <reference path="../_common/updateApiGatewayRouteName.ts" />

function applyRouteAssociationOnDeletedBehavior(): void {
    let source = association.getOtherEnd().typeReference.getType();
    let target = association.typeReference.getType();

    if (!source) {
        return;
    }

    updateApiGatewayRouteName(source, target);

    const HttpSettings = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
        
    let httpSettings = source.getStereotype(HttpSettings);
    if (httpSettings) {
        source.removeStereotype(HttpSettings);
    }

    let bodyField = source.getChildren("Body Field")[0];
    bodyField?.delete();
}

applyRouteAssociationOnDeletedBehavior();