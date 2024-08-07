/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function getUserIdentityType(): string {
    const stringTypeId: string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const guidTypeId: string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    const intTypeId: string = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    const longTypeId: string = "33013006-E404-48C2-AC46-24EF5A5774FD";

    //Setttings
    const basicAudtingSettingsId = "e51c0868-816d-432b-9cc3-c597fdb1ef0d";
    const userIdentityToAuditFieldId = "5f617e3b-b027-4b23-aeb5-5ee4c7968173";
    const identitySettingsId = "1045dea6-d28f-4ab8-9b5e-6f360035fdb6";
    const userIdTypeFieldId = "0ac959d1-dc9e-4403-ae44-2a75c53a1331";

    let userIdentityToAudit =  application.getSettings(basicAudtingSettingsId)?.getField(userIdentityToAuditFieldId)?.value ?? "user-id";
    if (userIdentityToAudit == "user-name"){
        return stringTypeId;
    }
    let userIdTypeName =  application.getSettings(identitySettingsId)?.getField(userIdTypeFieldId)?.value ?? "string";
    switch (userIdTypeName) {
        case 'long':
            return longTypeId;
        case 'int':
            return intTypeId;
        case 'guid':
            return guidTypeId;
        case 'string':
        default:
            return stringTypeId;
        }        
}

function ensureAttributes(element : MacroApi.Context.IElementApi, userIdentifier:string, enforceFieldTypes?: boolean){
    const dateTimeOffsetTypeId = "f1ba4df3-a5bc-427e-a591-4f6029f89bd7";

    ensureAttribute(element, userIdentifier, false, "CreatedBy", enforceFieldTypes);
    ensureAttribute(element, dateTimeOffsetTypeId, false, "CreatedDate", enforceFieldTypes);
    ensureAttribute(element, userIdentifier, true, "UpdatedBy", enforceFieldTypes);
    ensureAttribute(element, dateTimeOffsetTypeId, true, "UpdatedDate", enforceFieldTypes);
}

function ensureAttribute(element : MacroApi.Context.IElementApi, typeId: string, isNullable : boolean, name: string, enforceFieldType?: boolean) {
    let el = element.getChildren("Attribute").filter(x => x.getName() == name)[0];
    if (el == null) {
        el = createElement("Attribute", name, element.id);
        el.setMetadata("set-by-infrastructure", "true");
        el.typeReference.setIsNullable(isNullable);
        el.typeReference.setType(typeId);
    }
    if (enforceFieldType){
        el.typeReference.setIsNullable(isNullable);
        el.typeReference.setType(typeId);
    }
}
