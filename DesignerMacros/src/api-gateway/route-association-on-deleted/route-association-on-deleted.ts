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

    const sourceParent = source.getParent();
    
    let existingDtos = sourceParent
        .getChildren()
        .filter(child => child.hasMetadata("upstream") && child.getMetadata("upstream") === source.id);
    
    source.getChildren("DTO-Field").filter(field => field.getName() === "Body")[0]?.delete();

    for (let dto of existingDtos) {
        dto.delete();
    }
}

applyRouteAssociationOnDeletedBehavior();