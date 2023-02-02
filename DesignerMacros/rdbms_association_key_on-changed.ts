(async () => {

if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit"){
    return;
}

function requiresForeignKey(associationEnd)
{
    return isManyToVariantsOfOne(associationEnd) || isSelfReferencingZeroToOne(associationEnd);
}

function isManyToVariantsOfOne(associationEnd)
{
    return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
}

function isSelfReferencingZeroToOne(associationEnd)
{
    return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable && 
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
}

let sourceType = lookup(association.getOtherEnd().typeReference.typeId);
let targetType = lookup(association.typeReference.typeId);

if (sourceType && targetType) {

    if (requiresForeignKey(association) && sourceType.getMetadata("auto-manage-keys") != "false") {
        let pks = targetType.getChildren("Attribute").filter(x => x.getStereotype("Primary Key") != null);
        pks.forEach((pk, index) => {
        let fk = sourceType.getChildren().filter(x => x.getMetadata("association") == association.id)[index] ||
                 createElement("Attribute", "", sourceType.id);
            // This check to avoid a loop where the Domain script is updating the convensions and this keeps renaming it back.
            if (fk.getName().toLocaleLowerCase() !== `${ toCamelCase(association.getName())}${toPascalCase(pk.getName())}`.toLocaleLowerCase()) {
                fk.setName(`${ toCamelCase(association.getName())}${toPascalCase(pk.getName())}`);
            }
            fk.setMetadata("association", association.id);
            fk.setMetadata("is-managed-key", "true");
            if (!fk.hasStereotype("793a5128-57a1-440b-a206-af5722b752a6"))
                fk.addStereotype("793a5128-57a1-440b-a206-af5722b752a6");
            fk.typeReference.setType(pk.typeReference.typeId);
            fk.typeReference.setIsNullable(association.typeReference.isNullable);
        });
        sourceType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach((x, index) => {
            if (index >= pks.length)
                x.delete();
        })
        if (targetType.id !== sourceType.id && targetType.getMetadata("auto-manage-keys") != "false") {
            targetType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
    }
    else if (requiresForeignKey(association.getOtherEnd()) && targetType.getMetadata("auto-manage-keys") != "false") {
        let pks = sourceType.getChildren("Attribute").filter(x => x.getStereotype("Primary Key") != null);
        pks.forEach((pk, index) => {
        let fk = targetType.getChildren().filter(x => x.getMetadata("association") == association.id)[index] ||
                 createElement("Attribute", "", targetType.id);
            // This check to avoid a loop where the Domain script is updating the convensions and this keeps renaming it back.
            if (fk.getName().toLocaleLowerCase() !== `${ toCamelCase(association.getOtherEnd().getName()) }${toPascalCase(pk.getName())}`.toLocaleLowerCase()) {
                fk.setName(`${ toCamelCase(association.getOtherEnd().getName()) }${toPascalCase(pk.getName())}`);
            }
            fk.setMetadata("association", association.id);
            fk.setMetadata("is-managed-key", "true");
            if (!fk.hasStereotype("793a5128-57a1-440b-a206-af5722b752a6"))
                fk.addStereotype("793a5128-57a1-440b-a206-af5722b752a6");
            fk.typeReference.setType(pk.typeReference.typeId);
            fk.typeReference.setIsNullable(association.getOtherEnd().typeReference.isNullable);
        });
        targetType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach((x, index) => {
            if (index >= pks.length)
                x.delete();
        })
        if (sourceType.id !== targetType.id && sourceType.getMetadata("auto-manage-keys") != "false") {
            sourceType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
    }
    else { // many-to-many
        if (targetType.getMetadata("auto-manage-keys") != "false") {
            targetType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
        if (sourceType.getMetadata("auto-manage-keys") != "false") {
            sourceType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
    }
}

})();