/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

interface IFieldDiscrepancy {
    id: string;
    type: "NEW" | "DELETED" | "RENAMED" | "TYPE_CHANGED";
    dtoFieldId?: string;
    dtoFieldName: string;
    dtoFieldType?: string;
    entityAttributeId?: string;
    entityAttributeName: string;
    entityAttributeType?: string;
    mappingPath?: string[];
    reason?: string;
    icon: MacroApi.Context.IIcon;
    displayFunction?: (context: any) => MacroApi.Context.IDisplayTextComponent[];
}

interface ISyncContext {
    dtoElement: MacroApi.Context.IElementApi;
    entity: MacroApi.Context.IElementApi;
    actionAssociations: MacroApi.Context.IAssociationApi[];
    mappings: MacroApi.Context.IElementToElementMappingApi[];
    discrepancies: IFieldDiscrepancy[];
}

interface IFieldMapping {
    sourcePath: string[];
    targetPath: string[];
    sourceFieldId: string;
    targetAttributeId: string;
}

interface IDtoField {
    id: string;
    name: string;
    typeId?: string;
    typeDisplayText?: string;
    isMapped: boolean;
    mappedToAttributeId?: string;
    icon: MacroApi.Context.IIcon;
}

interface IEntityAttribute {
    id: string;
    name: string;
    typeId?: string;
    typeDisplayText?: string;
    icon: MacroApi.Context.IIcon;
}

interface IDiscrepancyDetails {
    oldValue?: string;
    newValue?: string;
    oldType?: string;
    newType?: string;
    details?: string;
}
