/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        element.getParent().getMetadata("auto-manage-keys") === "false" ||
        element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    if (element.getName().toLocaleLowerCase() == "id" &&
        element.hasStereotype("Primary Key") &&
        element.typeReference.getType()) {

        if (element.typeReference.getType()?.getName() != application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Type")?.value) {
            element.setMetadata("is-managed-key", "false");
        } else {
            element.setMetadata("is-managed-key", "true");
        }
    }

    const foreignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";
    let associationTarget = element.getStereotype(foreignKeyStereotypeId)?.getProperty("Association")?.getValue() as MacroApi.Context.IElementApi;

    if (associationTarget && element.getMetadata("association") != associationTarget.id) {
        element.setMetadata("association", associationTarget.id);
    }
}

execute();