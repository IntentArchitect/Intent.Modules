(async () => {

if (element.getPackage().specialization !== "Mongo Domain Package") {
    return;
}

const PrimaryKeyStereotypeId : string = "b99aac21-9ca4-467f-a3a6-046255a9eed6";

let idAttr = createElement("Attribute", "Id", element.id);
idAttr.typeReference.setType(getDefaultIdType());
idAttr.addMetadata("id-managed", "true");
idAttr.addStereotype(PrimaryKeyStereotypeId);

function getDefaultIdType() : string  {
    const StringTypeId : string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const GuidTypeId : string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    const IntTypeId : string = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    const LongTypeId : string = "33013006-E404-48C2-AC46-24EF5A5774FD";
    const MongoSettingId : string = "d5581fe8-7385-4bb6-88dc-8940e20ec1d4";
    
    switch (application.getSettings(MongoSettingId)?.getField("Id Type")?.value) {
        default:
            return StringTypeId;
        case "guid":
            return GuidTypeId;
        case "int":
            return IntTypeId;
        case "long":
            return LongTypeId;
    }
}

})();