/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />
type IElementApi = MacroApi.Context.IElementApi;

function updatePrimaryKey(element: IElementApi): void {
    if (element.specialization !== "Class") {
        return;
    }

    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let idAttr = element.getChildren("Attribute")
        .find(x => x.hasStereotype(primaryKeyStereotypeId));

    const isToOneRelationshipTarget = () => element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.getOtherEnd().typeReference.isCollection);
    if ((!isAggregateRoot(element) && isToOneRelationshipTarget()) || derivedFromTypeHasPk(element)) {
        if (idAttr != null) {
            idAttr.delete();
        }

        updateDerivedTypePks(element);
        return;
    }

    if (idAttr == null) {
        const classNameWithId = `${element.getName()}Id`.toLowerCase();

        idAttr = element.getChildren("Attribute")
            .find(attribute => {
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

    if (!idAttr.hasStereotype(primaryKeyStereotypeId)) {
        idAttr.addStereotype(primaryKeyStereotypeId);
    }

    updateDerivedTypePks(element);

    function derivedFromTypeHasPk(element: IElementApi): boolean {
        const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    
        return element.getAssociations("Generalization")
            .some(generalization => {
                if (!generalization.isTargetEnd()) {
                    return false;
                }
    
                const baseType = generalization.typeReference.getType();
                if (baseType.getChildren("Attribute").some(attribute => attribute.hasStereotype(primaryKeyStereotypeId))) {
                    return true;
                }
    
                return derivedFromTypeHasPk(baseType);
            });
    }

    function updateDerivedTypePks(element: IElementApi) {
        var derivedTypes = element.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());
    
        for (const derivedType of derivedTypes) {
            updatePrimaryKey(derivedType);
        }
    }
}
