/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

interface IFieldType {
    baseType?: string;
    isCollection: boolean;
    isNullable: boolean;
    displayText: string;
}

interface IFieldDiscrepancy {
    id: string;
    type: "NEW" | "DELETE" | "RENAME" | "CHANGE_TYPE";
    dtoFieldId?: string;
    dtoFieldName: string;
    dtoFieldType?: string;
    entityAttributeId?: string;
    entityAttributeName: string;
    entityAttributeType?: string;
    mappingPath?: string[];
    reason?: string;
    icon: MacroApi.Context.IIcon;

    displayFunction?: (context: MacroApi.Context.ISelectableTreeNode) => string | MacroApi.Context.IDisplayTextComponent[];

    fieldType?: IFieldType;
    
    // Type modifiers for source (DTO field)
    dtoTypeId?: string;
    dtoIsCollection?: boolean;
    dtoIsNullable?: boolean;
    
    // Type modifiers for target (Entity attribute)
    entityTypeId?: string;
    entityIsCollection?: boolean;
    entityIsNullable?: boolean;
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

// Association metadata for entity-to-entity composite relationships
interface IAssociationMetadata {
    id: string;
    associationName: string;
    sourceEntity: MacroApi.Context.IElementApi;          // The owning entity (e.g., Customer)
    targetEntity: MacroApi.Context.IElementApi;          // The owned entity (e.g., CustomerAddress)
    isCollection: boolean;                               // Whether it's a collection (array)
    isNullable: boolean;                                 // Whether it's optional
    targetAttributes: IEntityAttribute[];                // Fields of the target entity
    icon: MacroApi.Context.IIcon;
}

// DTO field that references an associated entity
interface IDtoAssociationField {
    id: string;
    name: string;
    typeId?: string;
    typeDisplayText?: string;
    isCollection: boolean;
    isNullable: boolean;
    icon: MacroApi.Context.IIcon;
}

// Extended tree node for structure-first architecture
interface IExtendedTreeNode extends MacroApi.Context.ISelectableTreeNode {
    // Node identity
    elementId: string;                      // The actual element this node represents
    elementType: string;                    // "DTO-Field" | "Attribute" | "Association" etc
    originalName: string;                   // Immutable original name
    originalType?: string;                  // Immutable original type display
    
    // Discrepancy tracking
    discrepancy?: IFieldDiscrepancy;        // If this node has a discrepancy
    hasDiscrepancies: boolean;              // If this OR any descendant has one
    
    // Navigation
    parentPath?: string[];                  // Path of element IDs to root
    
    // Context for nested comparisons
    contextDtoElement?: MacroApi.Context.IElementApi;     // The nested DTO type context
    contextEntityElement?: MacroApi.Context.IElementApi;  // The nested Entity type context
    
    // Type information for accurate comparison
    dtoTypeId?: string;                     // Type ID of DTO field
    dtoIsCollection?: boolean;              // Whether DTO field is collection
    dtoIsNullable?: boolean;                // Whether DTO field is nullable
    entityTypeId?: string;                  // Type ID of entity attribute
    entityIsCollection?: boolean;           // Whether entity attribute is collection
    entityIsNullable?: boolean;             // Whether entity attribute is nullable
}
