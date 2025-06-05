/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.Entities.BasicAuditing
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/DesignerMacros/src/azure-functions/openapi/class-on-applied/class-on-applied.ts
 */

function execute(element : MacroApi.Context.IElementApi) {
    const openapiStereotypeId = "d0dce09b-f93f-4b45-aeaf-106397bee14d";
    const openapiStereotypeSummaryId = "92bdc81f-b974-4a0d-afff-711a13fd1c3c";
    const openapiStereotypeDescriptionId = "58aa6166-08de-4810-ab34-8b6a09622a3d";
    const openapiStereotypeTagsId = "6f0ead5b-056b-4561-bb6e-e623ec73b677";

    element.getStereotype(openapiStereotypeId).getProperty(openapiStereotypeDescriptionId).setValue(getDescription(element));
    element.getStereotype(openapiStereotypeId).getProperty(openapiStereotypeSummaryId).setValue(getDescription(element));
    element.getStereotype(openapiStereotypeId).getProperty(openapiStereotypeTagsId).setValue(getTags(element));
}

function getDescription(element : MacroApi.Context.IElementApi): string {
    if (element.getComment()?.trim()) {
        return JSON.stringify(element.getComment());
    }

    return toSentenceCase(element.getName());
}

function getTags(element : MacroApi.Context.IElementApi): string {
    let result = "Default";

    const customSettings = element.getStereotype("Http Settings");
    const customRoute = customSettings?.getProperty("Route")?.getValue() as string || "";

    if (customRoute) {
        const index = customRoute.indexOf('/');
        result = index > -1 ? customRoute.substring(0, index) : customRoute;
    }

    return toPascalCase(result);
}
execute(element);