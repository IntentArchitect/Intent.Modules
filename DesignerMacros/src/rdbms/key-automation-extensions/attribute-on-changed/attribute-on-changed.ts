/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../common/getSurrogateKeyType.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        element.getParent().getMetadata(autoManageKeys) === "false" ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    if (element.getName().toLocaleLowerCase() == "id" &&
        element.hasStereotype("Primary Key") &&
        element.typeReference.getType()) {

        if (element.typeReference.getType()?.id != getSurrogateKeyType()) {
            element.setMetadata(isManagedKey, "false");
        } else {
            element.setMetadata(isManagedKey, "true");
        }
    }

    let associationTarget = element.getStereotype(foreignKeyStereotypeId)?.getProperty("Association")?.getValue() as MacroApi.Context.IElementApi;

    if (associationTarget && element.getMetadata("association") != associationTarget.id) {
        element.setMetadata("association", associationTarget.id);
    }
}

execute();