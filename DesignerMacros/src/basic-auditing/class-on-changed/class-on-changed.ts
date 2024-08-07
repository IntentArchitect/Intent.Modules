/// <reference path="../common/basic-auditing.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.Entities.BasicAuditing
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/DesignerMacros/src/basic-auditing/class-on-changed/class-on-changed.ts
 */


function execute(element : MacroApi.Context.IElementApi) {
    const auditStereotypeId = "796ec2fb-cdb9-4f8e-b096-a4f72c7a7f93";

    let userIdentifier = getUserIdentityType();

    if (element.hasStereotype(auditStereotypeId)) {
        ensureAttributes(element, userIdentifier);
    } else {
        removeAttribute(element, "CreatedBy");
        removeAttribute(element, "CreatedDate");
        removeAttribute(element, "UpdatedBy");
        removeAttribute(element, "UpdatedDate");
    }
}

function removeAttribute(element : MacroApi.Context.IElementApi, name: string) {
    let el = element.getChildren("Attribute").filter(x => x.getName() == name)[0];
    if (el == null || !el.hasMetadata("set-by-infrastructure")) {
        return;
    }

    el.delete();
}

execute(element);
