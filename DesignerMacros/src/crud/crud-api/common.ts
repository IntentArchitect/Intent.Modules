/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

const privateSettersOnly = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("0cf704e1-9a61-499a-bb91-b20717e334f5")?.value == "true";

interface ICrudCreationContext {
    element: IElementApi; 
    dialogOptions: ICrudCreationResult;
    primaryKeys: IAttributeWithMapPath[];
    hasPrimaryKey(): boolean;
}


async function notifyUserOfLimitations(entity: MacroApi.Context.IElementApi, dialogOptions: ICrudCreationResult){
    if ((privateSettersOnly && !hasConstructor(entity)) && dialogOptions.canCreate) {
        await dialogService.warn(
`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to private setters being enabled, and no constructor is present for entity '${entity.getName()}'.

To avoid this limitation in the future, either disable private setters or add a constructor element to the entity.`);
    } else if (!entityHasPrimaryKey(entity) && (dialogOptions.canDelete || dialogOptions.canQueryById || dialogOptions.canUpdate || dialogOptions.selectedDomainOperationIds.length > 0)  )  {
        await dialogService.warn(
`Partial CQRS Operation Creation.
Some CQRS operations were created successfully, but was limited due to no Primary Key on entity '${entity.getName()}'.

To avoid this limitation in the future, model a Primary Key on the entity.`);
    }
}

function hasConstructor(entity: MacroApi.Context.IElementApi): boolean {
    return entity.getChildren("Class Constructor").length > 0;
}

function entityHasPrimaryKey(entity: MacroApi.Context.IElementApi): boolean {
    const primaryKeys = DomainHelper.getPrimaryKeys(entity);
    return  primaryKeys.length > 0;
}



