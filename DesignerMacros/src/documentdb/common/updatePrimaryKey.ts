/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />

function updatePrimaryKey(element: MacroApi.Context.IElementApi) {
    const PrimaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let pk = element.getChildren("Attribute")
        .filter(x => x.hasStereotype(PrimaryKeyStereotypeId) || (x.hasMetadata("is-managed-key") && !x.hasMetadata("association")))[0];

    let isAggregate = isAggregateRoot(element);
    if (pk && (pk.hasStereotype(PrimaryKeyStereotypeId) && !isAggregate)) {
        pk.removeStereotype(PrimaryKeyStereotypeId);
        pk.setMetadata("is-managed-key", "false");
        return;
    }
    if (!isAggregate) {
        return;
    }

    let idAttr = pk || createElement("Attribute", "Id", element.id);
    if (!pk) {
        idAttr.setOrder(0);
        if (idAttr.typeReference == null) {
            throw new Error("typeReference is not defined");
        }

        idAttr.typeReference.setType(getDefaultIdType());
    }
    if (idAttr.getMetadata("is-managed-key") != "true") {
        idAttr.setMetadata("is-managed-key", "true");
    }
    if (!idAttr.hasStereotype(PrimaryKeyStereotypeId)) {
        idAttr.addStereotype(PrimaryKeyStereotypeId);
    }
}
