/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.DocumentDB
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/documentdb/association-on-delete/association-on-delete.ts
 */

function execute() {
    let targetClass = association.typeReference.getType();
    const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    if (!targetClass.getPackage().hasStereotype(documentStoreId)) {
        return;
    }

    updatePrimaryKey(targetClass);
    removeAssociatedForeignKeys(association);
}

function updatePrimaryKey(element: MacroApi.Context.IElementApi) {
    const PrimaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let pk = element.getChildren("Attribute")
        .filter(x => x.hasStereotype(PrimaryKeyStereotypeId) || (x.hasMetadata("is-managed-key") && !x.hasMetadata("association")))[0];

    let isAggregate = isAggregateRoot(element);
    if (pk && (pk.hasStereotype(PrimaryKeyStereotypeId) && !isAggregate)) {
        pk.removeStereotype(PrimaryKeyStereotypeId);
        pk.setMetadata("is-managed-key", "false");
        return;
    }
    if (!isAggregate) {
        return;
    }

    let idAttr = pk || createElement("Attribute", "Id", element.id);
    if (!pk) {
        idAttr.setOrder(0);
        idAttr.typeReference.setType(getDefaultIdType());
    }
    if (idAttr.getMetadata("is-managed-key") != "true") {
        idAttr.setMetadata("is-managed-key", "true");
    }
    if (!idAttr.hasStereotype(PrimaryKeyStereotypeId)) {
        idAttr.addStereotype(PrimaryKeyStereotypeId);
    }
}

function removeAssociatedForeignKeys(associationEnd: MacroApi.Context.IAssociationApi) {
    const ForeignKeyStereotypeId = "ced3e970-e900-4f99-bd04-b993228fe17d";
    let targetClass = associationEnd.typeReference.getType();
    let sourceClass = associationEnd.getOtherEnd().typeReference.getType();
    targetClass.getChildren()
        .filter(x => x.getMetadata("association") == associationEnd.id)
        .forEach(x => x.delete());
    sourceClass.getChildren()
        .filter(x => x.getMetadata("association") == associationEnd.id)
        .forEach(x => x.delete());
}

function isAggregateRoot(element: MacroApi.Context.IElementApi) {
    return !element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}

function getDefaultIdType(): string {
    const StringTypeId: string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const GuidTypeId: string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    const IntTypeId: string = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    const LongTypeId: string = "33013006-E404-48C2-AC46-24EF5A5774FD";
    const MongoSettingId: string = "d5581fe8-7385-4bb6-88dc-8940e20ec1d4";

    switch (application.getSettings(MongoSettingId)?.getField("Id Type")?.value) {
        default:
            return StringTypeId;
        case "guid":
            return GuidTypeId;
        case "int":
            return IntTypeId;
        case "long":
            return LongTypeId;
    }
}

execute();