/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/isFieldFromBody.ts" />

function syncDownstreamContract(apiGatewayRoute: MacroApi.Context.IElementApi, downstreamService: MacroApi.Context.IElementApi): void {
    if (apiGatewayRoute.specialization !== "Api Gateway Route") {
        throw Error(`Element "${apiGatewayRoute.id}, ${apiGatewayRoute.getName()}" is not of type Api Gateway Route`);
    }

    let downstreamContract = downstreamService.specialization === "Operation"
        ? downstreamService.getChildren("Parameter")
            .map(x => x.typeReference?.getType())
            .filter(x => x && x.specialization === "DTO")[0]
        : downstreamService;

    let downstreamFields = downstreamService.specialization === "Operation"
        ? downstreamContract?.getChildren("DTO-Field")
        : downstreamContract.getChildren("DTO-Field").filter(x => isFieldFromBody(downstreamService, x));
    
    if (!downstreamFields || downstreamFields.length === 0) {
        apiGatewayRoute.getChildren("Body Field")[0]?.delete();
        return;
    }

    let bodyField = apiGatewayRoute.getChildren("Body Field")[0];
    if (!bodyField) {
        bodyField = createElement("Body Field", "Body", apiGatewayRoute.id);
    }

    bodyField.typeReference?.setType(downstreamContract.id);
}