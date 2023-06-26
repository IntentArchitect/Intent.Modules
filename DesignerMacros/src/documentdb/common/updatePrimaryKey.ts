/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />

function updatePrimaryKey(element: MacroApi.Context.IElementApi) {
    if (element.specialization !== "Class") {
        return;
    }

    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let pk = element.getChildren("Attribute")
        .filter(x => x.hasStereotype(primaryKeyStereotypeId) || (x.hasMetadata("is-managed-key") && !x.hasMetadata("association")))[0];

    let idAttr = pk;
    if (idAttr == null) {
        idAttr = element.getChildren("Attribute").find(x => x.getName().toLowerCase() === "id");
    }

    if (idAttr == null) {
        idAttr = createElement("Attribute", "Id", element.id);
        idAttr.setOrder(0);
        if (idAttr.typeReference == null) throw new Error("typeReference is not defined");
        idAttr.typeReference.setType(getDefaultIdType());
    }

    if (idAttr.getMetadata("is-managed-key") != "true") {
        idAttr.setMetadata("is-managed-key", "true");
    }

    if (!idAttr.hasStereotype(primaryKeyStereotypeId)) {
        idAttr.addStereotype(primaryKeyStereotypeId);
    }
}
