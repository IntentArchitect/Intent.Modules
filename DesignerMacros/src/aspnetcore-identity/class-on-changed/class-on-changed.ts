/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

const identityUserStereotypeId = "efde089e-21e6-4da1-b086-72d7f6caf389";
const pkStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6"
const stringTypeId = "d384db9c-a279-45e1-801e-e4e8099625f2";
const textConstraintsStereotypeId = "6347286E-A637-44D6-A5D7-D9BE5789CA7A";

async function execute(): Promise<void> {
    const identityUserStereotype = element.getStereotype(identityUserStereotypeId);
    if (identityUserStereotype == null) {
        return;
    }

    let pkAttribute =
        element.getChildren("Attribute").find(x => x.hasStereotype(pkStereotypeId)) ??
        element.getChildren("Attribute").find(x => x.getName().toLowerCase() === "id");
    let textConstraintsStereotype = pkAttribute?.getStereotype(textConstraintsStereotypeId);
    if (pkAttribute != null &&
        pkAttribute.getName().toLowerCase() === "id" &&
        pkAttribute.hasStereotype(pkStereotypeId) &&
        pkAttribute.typeReference?.typeId === stringTypeId &&
        textConstraintsStereotype?.getProperty("SQL Data Type")?.getValue() === "DEFAULT" &&
        textConstraintsStereotype.getProperty("MaxLength")?.getValue() === 450
    ) {
        return;
    }

    if (dialogService?.info != null) {
        await dialogService.info("When the \"Identity User\" stereotype is applied to a class, it must have an attribute with all the following characteristics:\n" +
        "- A \"Primary Key\" stereotype applied\n" +
        "- A name of \"id\"\n" +
        "- Its type set to \"string\"\n" +
        "- The \"Text Constraints\" stereotype applied to it\n" +
        "- Its \"Text Constraints\" stereotype's \"SQL Data Type\" property must be set to \"DEFAULT\"\n" +
        "- Its \"Text Constraints\" stereotype's \"MaxLength\" property must be set to \"450\"\n" +
        "\n" +
        "This class will now be updated to meet these requirements.");
    }

    if (pkAttribute == null) {
        pkAttribute = createElement("Attribute", "id", element.id);
        pkAttribute.setOrder(0);
    }

    if (pkAttribute.getName().toLowerCase() !== "id") {
        pkAttribute.setName("id");
    }

    if (!pkAttribute.hasStereotype(pkStereotypeId)) {
        pkAttribute.addStereotype(pkStereotypeId);
    }

    pkAttribute.typeReference.setType(stringTypeId);

    textConstraintsStereotype = pkAttribute?.getStereotype(textConstraintsStereotypeId);
    if (textConstraintsStereotype == null) {
        pkAttribute.addStereotype(textConstraintsStereotypeId);
        textConstraintsStereotype = pkAttribute.getStereotype(textConstraintsStereotypeId);
    }

    textConstraintsStereotype.getProperty("SQL Data Type").setValue("DEFAULT");
    textConstraintsStereotype.getProperty("MaxLength").setValue(450);
 }

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Identity
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/aspnetcore-identity/class-on-changed/class-on-changed.ts
 */

//await execute();
