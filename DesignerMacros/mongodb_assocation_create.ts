(async () => {
if (!association) { return; }

let sourceEnd = association.getOtherEnd().typeReference;
if (sourceEnd.getType().getPackage().specialization !== "Mongo Domain Package") {
    return;
}

sourceEnd.setIsCollection(false);
sourceEnd.setIsNullable(false);

})();