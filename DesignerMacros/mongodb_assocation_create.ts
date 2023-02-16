(async () => {
if (!association) { return; }

let sourceEnd = association.getOtherEnd().typeReference;
if (sourceEnd.getType().getPackage().specialization !== "Mongo Domain Package") {
    return;
}
//let targetEnd = association.typeReference;
sourceEnd.setIsCollection(false);
sourceEnd.setIsNullable(false);
//targetEnd.setIsCollection(false);
//targetEnd.setIsNullable(false);
})();