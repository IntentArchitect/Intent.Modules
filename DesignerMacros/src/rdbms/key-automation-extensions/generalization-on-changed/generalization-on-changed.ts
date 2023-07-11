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
    var pks = sourceType.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key") && x.getMetadata("is-managed-key") == "true");
    pks.forEach(x => x.delete());
}

})();