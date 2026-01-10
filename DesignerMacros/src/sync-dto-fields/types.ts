/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

interface IFieldDiscrepancy {
    id: string;
    type: "NEW" | "DELETE" | "RENAME" | "CHANGE_TYPE";
    
    sourceParentId?: string;
    sourceFieldId?: string;
    sourceFieldName: string;
    sourceFieldTypeName?: string;
    sourceTypeId?: string;
    sourceIsCollection?: boolean;
    sourceIsNullable?: boolean;

    targetAttributeId?: string;
    targetAttributeName: string;
    targetAttributeTypeName?: string;
    targetTypeId?: string;
    targetIsCollection?: boolean;
    targetIsNullable?: boolean;
    
    mappingPath?: string[];
    reason?: string;
    icon: MacroApi.Context.IIcon;

    displayFunction?: (context: MacroApi.Context.ISelectableTreeNode) => string | MacroApi.Context.IDisplayTextComponent[];
}

interface IFieldMapping {
    sourcePath: string[];
    targetPath: string[];
    sourceFieldId: string;
    targetAttributeId: string;
    mappingType?: string;
    mappingTypeId?: string;
}

interface IPathTemplate {
    sourceDepth: number;
    targetDepth: number;
    sourceElementTypes: string[];
    targetElementTypes: string[];
    signature: string;
    mappingTypeId: string;
    mappingTypeName: string;
}

interface IEntityAttribute {
    id: string;
    name: string;
    typeId?: string;
    typeDisplayText?: string;
    icon: MacroApi.Context.IIcon;
    isManagedKey?: boolean;
    hasPrimaryKeyStereotype?: boolean;
}

// Hierarchical structures for generic parameter and field representation
interface IFieldNode {
    id: string;
    name: string;
    type: "Primitive" | "DTO" | "Complex";
    typeId?: string;
    typeDisplayText?: string;
    icon: MacroApi.Context.IIcon;
    isMapped?: boolean;
    mappedToId?: string;
    children?: IFieldNode[];  // For nested structures (DTO fields, nested objects)
}

interface IParameterNode {
    id: string;
    name: string;
    type: "Primitive" | "DTO" | "Complex";
    typeId?: string;
    typeDisplayText?: string;
    icon: MacroApi.Context.IIcon;
    children?: IFieldNode[];  // Only for DTO or Complex types
    isMapped?: boolean;
    mappedToId?: string;
}

interface IExtendedTreeNode extends MacroApi.Context.ISelectableTreeNode {
    elementId: string;
    elementType: string;
    originalName: string;
    originalType?: string;
    discrepancy?: IFieldDiscrepancy;
    hasDiscrepancies: boolean;

    elementParentId?: string;
}
