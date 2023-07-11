/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="constants.ts" />
type IElementApi = MacroApi.Context.IElementApi;

function updatePrimaryKeys(element: IElementApi): void {
    if (element.getMetadata("auto-manage-keys") === "false" ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        updateDerivedTypePks(element);
        return;
    }

    const pkAttributes = element.getChildren("Attribute").filter(x => x.hasStereotype(primaryKeyStereotypeId));

    if (derivedFromTypeHasPk(element)) {
        for (const pkAttribute of pkAttributes.filter(x => x.getMetadata(isManagedKey) === "true")) {
            pkAttribute.setMetadata(isBeingDeletedByScript, "true");
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
    pkAttribute.setMetadata(isManagedKey, "true");

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

    function getSurrogateKeyType(): string {
        const commonTypes = {
            guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
            long: "33013006-E404-48C2-AC46-24EF5A5774FD",
            int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74"
        };
        const javaTypes = {
            long: "e9e575eb-f8de-4ce4-9838-2d09665a752d",
            int: "b3e5cb3b-8a26-4346-810b-9789afa25a82"
        };

        const typeNameToIdMap = new Map();
        typeNameToIdMap.set("guid", commonTypes.guid);
        typeNameToIdMap.set("int", lookup(javaTypes.int) != null ? javaTypes.int : commonTypes.int);
        typeNameToIdMap.set("long", lookup(javaTypes.long) != null ? javaTypes.long : commonTypes.long);

        let typeName = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Type")?.value ?? "int";
        if (typeNameToIdMap.has(typeName)) {
            return typeNameToIdMap.get(typeName);
        }

        return typeNameToIdMap.get("guid");
    }
}