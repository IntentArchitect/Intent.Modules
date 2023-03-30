(async () => {

if (element.getParent().getPackage().specialization !== "Mongo Domain Package") {
    return;
}
const PrimaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
if (!element.hasMetadata("id-managed")) {
    if (!element.hasStereotype(PrimaryKeyStereotypeId)) {
        element.removeStereotype(PrimaryKeyStereotypeId);
    }    
    return;
}

if (!element.hasStereotype(PrimaryKeyStereotypeId)) {
    element.addStereotype(PrimaryKeyStereotypeId);
}

})();