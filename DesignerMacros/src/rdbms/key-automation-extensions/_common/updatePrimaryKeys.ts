/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="constants.ts" />

function updatePrimaryKeys(element: IElementApi): void {
    if (element.getMetadata(metadataKey.autoManageKeys) === "false" ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        updateDerivedTypePks(element);
        return;
    }

    const pkAttributes = element.getChildren("Attribute").filter(x => x.hasStereotype(primaryKeyStereotypeId));

    if (derivedFromTypeHasPk(element)) {
        for (const pkAttribute of pkAttributes.filter(x => x.getMetadata(metadataKey.isManagedKey) === "true")) {
            pkAttribute.setMetadata(metadataKey.isBeingDeletedByScript, "true");
            pkAttribute.delete();
        }

        updateDerivedTypePks(element);
        return;
    }

    if (pkAttributes.length > 0) {
        updateDerivedTypePks(element);
        return;
    }

    const pkAttribute = createElement("Attribute", "id", element.id);
    pkAttribute.setOrder(0);
    pkAttribute.typeReference.setType(getSurrogateKeyType());
    pkAttribute.addStereotype(primaryKeyStereotypeId);
    pkAttribute.setMetadata(metadataKey.isManagedKey, "true");

    function derivedFromTypeHasPk(element: IElementApi): boolean {
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

    function updateDerivedTypePks(element: IElementApi): void {
        var derivedTypes = element.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());

        for (const derivedType of derivedTypes) {
            updatePrimaryKeys(derivedType);
        }
    }
}