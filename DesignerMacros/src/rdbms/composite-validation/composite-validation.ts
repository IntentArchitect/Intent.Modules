/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/domainHelper.ts" />

// Check composite only has 1 owner
 
const dbSettingsId = "ac0a788e-d8b3-4eea-b56d-538608f1ded9";

function validateDomainEntity(entity:IElementApi):string | null{

    if (!entity.getPackage().hasStereotype("Relational Database")) {
        return null;
    }

    if (isCosmosDbProvider() || DomainHelper.isAggregateRoot(element)) { 
        return null; 
    }

    let owners = DomainHelper.getOwnersRecursive(entity);
    if (owners.length > 1){
        const uniqueIds = new Set(owners.map(item => item.id));
        if (uniqueIds.size !== 1) {
            let ownersDescription = owners.map(item => item.getName()).join(", ");
            return `Entity has multiple owners. The entity '${entity.getName()}' has multiple owners. [${ownersDescription}].
Compositional entities (black diamond) must have 1 owner. Please adjust the associations accordingly.`;
        }
    }
    return null;
}


function isCosmosDbProvider() {
    return application.getSettings(dbSettingsId)
        ?.getField("Database Provider")
        ?.value == "cosmos";
}

/**
 * Used by Intent.EntityFrameworkCore
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/rdbms/composite-validation/composite-validation.ts
 */
//return validateDomainEntity(element);
validateDomainEntity(element);
