/// <reference path="../_common/isFieldFromBody.ts" />
/// <reference path="../_common/getMethodAndRoute.ts" />
/// <reference path="../_common/updateApiGatewayRouteName.ts" />

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

function syncDownstreamContract(source: MacroApi.Context.IElementApi, target: MacroApi.Context.IElementApi): void {
    let downstreamFields = target.getChildren("DTO-Field").filter(x => isFieldFromBody(target, x));
    
    if (!downstreamFields || downstreamFields.length === 0) {
        return;
    }
    
    const sourceParent = source.getParent();
    
    let existingDto = sourceParent
        .getChildren()
        .filter(child => child.hasMetadata("upstream") && child.getMetadata("upstream") === source.id)[0];
    
    let dto = existingDto;
    if (!dto) {
        const dtoName = target.getName();
        dto = createElement("DTO", dtoName, sourceParent.id);
        dto.addMetadata("upstream", source.id);
    }

    for (let downstreamField of downstreamFields) {
        let field = dto.getChildren("DTO-Field").filter(field => field.getName() === downstreamField.getName())[0];
        if (!field) {
            field = createElement("DTO-Field", downstreamField.getName(), dto.id);
            field.typeReference?.setType(downstreamField.typeReference.getTypeId());
        }
    }

    for (let upstreamField of dto.getChildren("DTO-Field")) {
        if (!downstreamFields.some(field => upstreamField.getName() === field.getName())) {
            upstreamField.delete();
        }
    }
    
    let bodyField = source.getChildren("DTO-Field").filter(x => x.getName() === "Body")[0];
    if (!bodyField) {
        bodyField = createElement("DTO-Field", "Body", source.id);
    }
    
    bodyField.typeReference?.setType(dto.id);
}

applyRouteAssociationOnChangedBehavior();