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
    displayFunction?: () => MacroApi.Context.IDisplayTextComponent[];
}

interface IFieldMapping {
    sourcePath: string[];
    targetPath: string[];
    sourceFieldId: string;
    targetAttributeId: string;
    mappingType?: string;
}

interface IPathTemplate {
    sourceDepth: number;
    targetDepth: number;
    sourceElementTypes: string[];
    targetElementTypes: string[];
    signature: string;
    mappingType: string;
}

interface IMappingContext {
    sourceRoot: MacroApi.Context.IElementApi;
    targetRoot: MacroApi.Context.IElementApi;
    mappingType: string;
    pathTemplate: IPathTemplate | null;
    requiresIntermediateElements: boolean;
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
    isManagedKey?: boolean;
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

interface IMappableElement {
    element: MacroApi.Context.IElementApi;  // The Operation, Command, Query, etc.
    parameters: IParameterNode[];            // All parameters in the element
    targetEntity: MacroApi.Context.IElementApi | null;  // The target (Class) being mapped to
}
