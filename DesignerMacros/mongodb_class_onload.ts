(async () => {

if (element.getPackage().specialization !== "Mongo Domain Package") {
    return;
}

const PrimaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
let pk = element.getChildren("Attribute").filter(x => x.hasMetadata("id"))[0];
if (pk && pk.hasStereotype(PrimaryKeyStereotypeId)) {
    return;
}

const StringTypeId = "d384db9c-a279-45e1-801e-e4e8099625f2";
const GuidTypeId = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
let idAttr = pk || createElement("Attribute", "Id", element.id);

idAttr.setOrder(0);
idAttr.typeReference.setType(isAggregateRoot(element) ? StringTypeId : GuidTypeId);
if (!idAttr.hasMetadata("id")) {
    idAttr.addMetadata("id", "true");
}
if (!idAttr.hasStereotype(PrimaryKeyStereotypeId)) {
    idAttr.addStereotype(PrimaryKeyStereotypeId);
}

function isAggregateRoot(element) {
    return !element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}

})();