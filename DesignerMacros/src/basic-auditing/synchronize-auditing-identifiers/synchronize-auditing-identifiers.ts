/// <reference path="../common/basic-auditing.ts" />

/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.Entities.BasicAuditing
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/DesignerMacros/src/basic-auditing/synchronize-auditing-identifiers/synchronize-auditing-identifiers.ts
 */

function execute(element : MacroApi.Context.IElementApi) {

    let userIdentifier = getUserIdentityType();

    let classes = lookupTypesOf("Class").filter(x => x.hasStereotype("Basic Auditing"));
    classes.forEach( audit => {
        ensureAttributes(audit, userIdentifier, true);
    });
}

execute(element);
