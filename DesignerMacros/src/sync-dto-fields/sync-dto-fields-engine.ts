/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

/**
 * Structure-first DTO field synchronization engine
 * 
 * Current focus: Build correct tree hierarchy
 * (Discrepancy detection deferred until structure is verified)
 */
class FieldSyncEngine {

    private lastStructureTree: IExtendedTreeNode | null = null;

    /**
     * Main entry point: Build and display tree structure
     */
    public analyzeFieldDiscrepancies(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[],
        excludedEntityAttributeIds: Set<string>,
        sourceElement?: MacroApi.Context.IElementApi
    ): IFieldDiscrepancy[] {
        console.log(`[BUILD] Starting structure tree for DTO: ${dtoElement.getName()}`);
        console.log(`[BUILD] ├─ Entity: ${entity.getName()}`);
        
        // Build structure and store it
        // Pass sourceElement (the original Operation/Command/DTO) so we can show Operation params if needed
        this.buildStructureTree(dtoElement, entity, mappings, sourceElement);
        
        // Return empty discrepancies for now - we'll visualize the tree in the UI
        return [];
    }

    /**
     * Build the complete natural tree structure from DTO/entity
     */
    private buildStructureTree(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[],
        sourceElement?: MacroApi.Context.IElementApi
    ): void {
        const rootNode: IExtendedTreeNode = {
            id: dtoElement.id,
            label: dtoElement.getName(),
            specializationId: "structure-root",
            elementId: dtoElement.id,
            elementType: "DTO",
            originalName: dtoElement.getName(),
            originalType: undefined,
            hasDiscrepancies: true,  // Set to true so it shows in tree
            isExpanded: true,
            isSelected: false,
            icon: dtoElement.getIcon(),
            children: []
        };

        // If sourceElement is an Operation, show its parameters first
        if (sourceElement && sourceElement.specialization === "Operation") {
            const operationParams = sourceElement.getChildren("Parameter");
            console.log(`[BUILD] ├─ Operation parameters (${operationParams.length}):`);
            
            for (const param of operationParams) {
                const paramTypeRef = param.typeReference;
                const paramTypeName = paramTypeRef?.display || "Unknown";
                
                // Skip the DTO parameter itself - we'll show its fields instead
                if (paramTypeRef && paramTypeRef.isTypeFound()) {
                    const paramType = paramTypeRef.getType() as MacroApi.Context.IElementApi;
                    if (paramType && paramType.id === dtoElement.id) {
                        console.log(`[BUILD] │  ├─ (Skipping DTO parameter: ${param.getName()})`);
                        continue;  // Skip, we handle it separately
                    }
                }
                
                console.log(`[BUILD] │  ├─ ${param.getName()}: ${paramTypeName}`);
                
                const paramNode: IExtendedTreeNode = {
                    id: param.id,
                    label: `${param.getName()}: ${paramTypeName}`,
                    specializationId: "structure-operation-param",
                    elementId: param.id,
                    elementType: "Operation-Parameter",
                    originalName: param.getName(),
                    originalType: paramTypeName,
                    hasDiscrepancies: true,
                    isExpanded: true,
                    isSelected: false,
                    icon: param.getIcon(),
                    children: []
                };
                
                rootNode.children!.push(paramNode);
            }
        }

        // Add DTO fields (direct properties)
        const childType = inferSourceElementChildType(dtoElement);
        const dtoChildren = dtoElement.getChildren(childType);
        console.log(`[BUILD] ├─ DTO fields (${dtoChildren.length}):`);

        for (const dtoChild of dtoChildren) {
            const fieldTypeRef = dtoChild.typeReference;
            const fieldTypeName = fieldTypeRef?.display || "Unknown";
            console.log(`[BUILD] │  ├─ ${dtoChild.getName()}: ${fieldTypeName}`);
            
            const fieldNode: IExtendedTreeNode = {
                id: dtoChild.id,
                label: `${dtoChild.getName()}: ${fieldTypeName}`,
                specializationId: "structure-dto-field",
                elementId: dtoChild.id,
                elementType: "DTO-Field",
                originalName: dtoChild.getName(),
                originalType: fieldTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: dtoChild.getIcon(),
                children: []
            };

            // Check if this is a complex type and add nested fields
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType() as MacroApi.Context.IElementApi;
                if (fieldType && (fieldType.specialization === "DTO" || fieldType.specialization === "Class")) {
                    this.addNestedDtoFields(fieldNode, fieldType);
                }
            }
            
            rootNode.children!.push(fieldNode);
        }

        // NOTE: Entity attributes and associations are used for discrepancy detection
        // but we don't show them in the tree visualization for now
        // Only show the DTO fields structure
        
        // Store the built tree for later use
        this.lastStructureTree = rootNode;
    }

    private addNestedDtoFields(parentNode: IExtendedTreeNode, fieldType: MacroApi.Context.IElementApi): void {
        const childType = inferSourceElementChildType(fieldType);
        const children = fieldType.getChildren(childType);
        
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = childTypeRef?.display || "Unknown";
            console.log(`[BUILD] │  │  ├─ ${child.getName()}: ${childTypeName}`);
            
            const childNode: IExtendedTreeNode = {
                id: child.id,
                label: `${child.getName()}: ${childTypeName}`,
                specializationId: "structure-nested-field",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: []
            };
            
            // Recursively add nested fields
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType() as MacroApi.Context.IElementApi;
                if (nestedType && (nestedType.specialization === "DTO" || nestedType.specialization === "Class")) {
                    this.addNestedDtoFieldsWithDepth(childNode, nestedType, 3);
                }
            }
            
            parentNode.children!.push(childNode);
        }
    }

    private addNestedDtoFieldsWithDepth(parentNode: IExtendedTreeNode, fieldType: MacroApi.Context.IElementApi, depth: number): void {
        const childType = inferSourceElementChildType(fieldType);
        const children = fieldType.getChildren(childType);
        const indent = Array(depth).fill("│  ").join("");
        
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = childTypeRef?.display || "Unknown";
            console.log(`[BUILD] ${indent}├─ ${child.getName()}: ${childTypeName}`);
            
            const childNode: IExtendedTreeNode = {
                id: child.id,
                label: `${child.getName()}: ${childTypeName}`,
                specializationId: "structure-nested-field",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: []
            };
            
            // Recursively add nested fields
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType() as MacroApi.Context.IElementApi;
                if (nestedType && (nestedType.specialization === "DTO" || nestedType.specialization === "Class")) {
                    this.addNestedDtoFieldsWithDepth(childNode, nestedType, depth + 1);
                }
            }
            
            parentNode.children!.push(childNode);
        }
    }

    /**
     * Convert structure tree to display nodes
     */
    public buildHierarchicalTreeNodes(
        sourceElement: MacroApi.Context.IElementApi,
        dtoElement: MacroApi.Context.IElementApi,
        discrepancies: IFieldDiscrepancy[]
    ): MacroApi.Context.ISelectableTreeNode[] {
        // Return the children of the structure tree (not the root itself, since the dialog creates its own root)
        if (this.lastStructureTree && this.lastStructureTree.children) {
            const result: MacroApi.Context.ISelectableTreeNode[] = [];
            for (const child of this.lastStructureTree.children as IExtendedTreeNode[]) {
                const displayNode = this.convertNodeToDisplay(child);
                result.push(displayNode);
            }
            return result;
        }
        
        return [];
    }

    private convertNodeToDisplay(node: IExtendedTreeNode): MacroApi.Context.ISelectableTreeNode {
        const displayNode: MacroApi.Context.ISelectableTreeNode = {
            id: node.id,
            label: node.label,
            specializationId: node.specializationId,
            isExpanded: node.isExpanded,
            isSelected: node.isSelected,
            icon: node.icon,
            children: []
        };

        // Recursively convert children
        if (node.children && node.children.length > 0) {
            for (const child of node.children as IExtendedTreeNode[]) {
                const childDisplayNode = this.convertNodeToDisplay(child);
                displayNode.children!.push(childDisplayNode);
            }
        }

        return displayNode;
    }

    /**
     * Apply sync actions (placeholder for later)
     */
    public applySyncActions(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        selectedDiscrepancies: IFieldDiscrepancy[],
        associations: MacroApi.Context.IAssociationApi[]
    ): void {
        // Not implemented yet
    }
}
