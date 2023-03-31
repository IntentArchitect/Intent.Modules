(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
if (!association) { return; }

let sourceEnd = association.getOtherEnd().typeReference;
if (sourceEnd.getType().getPackage().specialization !== "Mongo Domain Package") {
    return;
}

sourceEnd.setIsCollection(false);
sourceEnd.setIsNullable(false);

})();