/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

function execute(): void {
    if (element.getPackage().specialization !== "Domain Package" ||
        !element.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    const foreignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";
    let associationTarget = element.getStereotype(foreignKeyStereotypeId)?.getProperty("Association Target")?.getValue() as MacroApi.Context.IElementApi;
    let existingAssociation = element.getMetadata("association");

    if (!associationTarget && existingAssociation) {
        if (!element.hasStereotype(foreignKeyStereotypeId)) {
            element.addStereotype(foreignKeyStereotypeId);
        }
        let stereotype = element.getStereotype(foreignKeyStereotypeId);
        stereotype.getProperty("Association").setValue(existingAssociation);
    }

    if (existingAssociation && !element.getMetadata("fk-original-name")) {
        element.setMetadata("fk-original-name", element.getName());
    }
}

execute();