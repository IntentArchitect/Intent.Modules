/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function getDefaultIdType(): string {
    const stringTypeId: string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const guidTypeId: string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    const intTypeId: string = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    const longTypeId: string = "33013006-E404-48C2-AC46-24EF5A5774FD";
    const documentDbSettingsId: string = "d5581fe8-7385-4bb6-88dc-8940e20ec1d4";

    switch (application.getSettings(documentDbSettingsId)?.getField("Id Type")?.value) {
        default:
            return stringTypeId;
        case "guid":
            return guidTypeId;
        case "int":
            return intTypeId;
        case "long":
            return longTypeId;
    }
}