(async () => {

if (element.getPackage().specialization !== "Mongo Domain Package") {
    return;
}

const StringTypeId : string = "d384db9c-a279-45e1-801e-e4e8099625f2";
const GuidTypeId : string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
const PrimaryKeyStereotypeId : string = "b99aac21-9ca4-467f-a3a6-046255a9eed6";

let idAttr = createElement("Attribute", "Id", element.id);
idAttr.typeReference.setType(isAggregateRoot(element) ? StringTypeId : GuidTypeId);
idAttr.addMetadata("id", "true");
idAttr.addStereotype(PrimaryKeyStereotypeId);

function isAggregateRoot(element : MacroApi.Context.IElementApi) : boolean {
    return ! element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}

})();