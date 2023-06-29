/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />

function updatePrimaryKey(element: MacroApi.Context.IElementApi): void {
    if (element.specialization !== "Class") {
        return;
    }

    const isManagedKey = "is-managed-key";
    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let idAttr = element.getChildren("Attribute")
        .find(x => x.hasStereotype(primaryKeyStereotypeId) || (x.hasMetadata(isManagedKey) && !x.hasMetadata("association")));

    const isToOneRelationshipTarget = () => element.getAssociations("Association").some(x => x.isSourceEnd() && !x.getOtherEnd().typeReference.isCollection);
    if (!isAggregateRoot(element) && isToOneRelationshipTarget()) {
        if (idAttr != null) {
            idAttr.delete();
        }

        return;
    }

    if (idAttr == null) {
        const classNameWithId = `${element.getName()}Id`.toLowerCase();

        idAttr = element.getChildren("Attribute").find(attribute => {
            const attributeName = attribute.getName().toLowerCase();
            
            return attributeName === "id" || attributeName === classNameWithId;
        });
    }

    if (idAttr == null) {
        idAttr = createElement("Attribute", "Id", element.id);
        idAttr.setOrder(0);
        if (idAttr.typeReference == null) throw new Error("typeReference is not defined");
        idAttr.typeReference.setType(getDefaultIdType());
    }

    idAttr.setMetadata(isManagedKey, "true");

    if (!idAttr.hasStereotype(primaryKeyStereotypeId)) {
        idAttr.addStereotype(primaryKeyStereotypeId);
    }
}
