/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/isFieldFromBody.ts" />

function syncDownstreamContract(apiGatewayRoute: MacroApi.Context.IElementApi, downstreamService: MacroApi.Context.IElementApi): void {
    if (apiGatewayRoute.specialization !== "Api Gateway Route") {
        throw Error(`Element "${apiGatewayRoute.id}, ${apiGatewayRoute.getName()}" is not of type Api Gateway Route`);
    }
    
    let downstreamFields = downstreamService.getChildren("DTO-Field").filter(x => isFieldFromBody(downstreamService, x));
    
    if (!downstreamFields || downstreamFields.length === 0) {
        return;
    }
    
    const sourceParent = apiGatewayRoute.getParent();
    
    let existingDto = sourceParent
        .getChildren()
        .filter(child => child.hasMetadata("upstream") && child.getMetadata("upstream") === apiGatewayRoute.id)[0];
    
    let dto = existingDto;
    if (!dto) {
        const dtoName = downstreamService.getName();
        dto = createElement("DTO", dtoName, sourceParent.id);
        dto.addMetadata("upstream", apiGatewayRoute.id);
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
    
    let bodyField = apiGatewayRoute.getChildren("DTO-Field").filter(x => x.getName() === "Body")[0];
    if (!bodyField) {
        bodyField = createElement("DTO-Field", "Body", apiGatewayRoute.id);
    }
    
    bodyField.typeReference?.setType(dto.id);
}