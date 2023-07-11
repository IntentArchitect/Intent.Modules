/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

(async () => {

if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit"){
    return;
}

let sourceEnd = association.getOtherEnd().typeReference;
if (sourceEnd.getType().getPackage().specialization !== "Domain Package") {
    return;
}

let sourceType = association.getOtherEnd().typeReference.getType();
let targetType = association.typeReference.getType();

if (sourceType.getMetadata("auto-manage-keys") == "false") {
    return;
}

if (sourceType && targetType) {
    var pks = sourceType.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
    if (pks.length == 0) {
        let pk = createElement("Attribute", "id", sourceType.id.toString());
        pk.typeReference.setType(getSurrogateKeyType());
        pk.addStereotype("b99aac21-9ca4-467f-a3a6-046255a9eed6");
        pk.setMetadata("is-managed-key", "true");
        pk.setOrder(0);
    }
}

function getSurrogateKeyType() : string {
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
})();