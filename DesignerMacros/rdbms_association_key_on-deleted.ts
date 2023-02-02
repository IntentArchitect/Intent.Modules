(async () => {

let sourceType = lookup(association.getOtherEnd().typeReference.typeId);
let targetType = lookup(association.typeReference.typeId);
if (targetType && targetType.getMetadata("auto-manage-keys") != "false") {
    targetType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
}
if (sourceType && sourceType.getMetadata("auto-manage-keys") != "false") {
    sourceType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
}

})();