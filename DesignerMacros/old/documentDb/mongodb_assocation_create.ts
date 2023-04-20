(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
if (!association) { return; }

let sourceEnd = association.getOtherEnd().typeReference;
const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
if (!sourceEnd.getType().getPackage().hasStereotype(documentStoreId)) {
    return;
}

sourceEnd.setIsCollection(false);
sourceEnd.setIsNullable(false);

})();