(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
if (element.getParent().getPackage().specialization !== "Mongo Domain Package") {
    return;
}
const PrimaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
const ForeignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";

if (!element.hasMetadata("is-managed-key")) {
    if (element.hasStereotype(PrimaryKeyStereotypeId)) {
        element.removeStereotype(PrimaryKeyStereotypeId);
    }
    if (element.hasStereotype(ForeignKeyStereotypeId)) {
        element.removeStereotype(ForeignKeyStereotypeId);
    }
    
    return;
}

if (!element.hasMetadata("association") && !element.hasStereotype(PrimaryKeyStereotypeId)) {
    element.addStereotype(PrimaryKeyStereotypeId);
}

if (element.hasMetadata("association") && !element.hasStereotype(ForeignKeyStereotypeId)) {
    element.addStereotype(ForeignKeyStereotypeId);
}

})();