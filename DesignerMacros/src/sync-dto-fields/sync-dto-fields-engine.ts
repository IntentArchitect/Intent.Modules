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
     * Main entry point: Build, annotate, and prune tree structure
     * Three-phase approach: BUILD -> ANNOTATE -> PRUNE
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
        
        // Phase 1: Build complete structure and store it
        this.buildStructureTree(dtoElement, entity, mappings, sourceElement);
        
        // Phase 2: Annotate discrepancies (including nested and operation parameters)
        const discrepancies: IFieldDiscrepancy[] = [];
        
        // Check operation parameters if present
        if (sourceElement && sourceElement.specialization === "Operation") {
            this.detectOperationParameterDiscrepancies(sourceElement, dtoElement, entity, mappings, discrepancies);
        }
        
        // Check DTO fields recursively
        this.detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, 0);
        
        this.annotateTreeWithDiscrepancies(this.lastStructureTree!, discrepancies);
        console.log(`[ANALYZE] └─ Total discrepancies detected: ${discrepancies.length}`);
        
        // Phase 3: Prune branches without discrepancies
        this.pruneTreeWithoutDiscrepancies(this.lastStructureTree!);
        
        return discrepancies;
    }

    /**
     * Check operation parameters for discrepancies
     */
    private detectOperationParameterDiscrepancies(
        sourceElement: MacroApi.Context.IElementApi,
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[],
        discrepancies: IFieldDiscrepancy[]
    ): void {
        const operationParams = sourceElement.getChildren("Parameter");
        console.log(`[ANALYZE-PARAMS] Checking ${operationParams.length} operation parameters`);
        
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map<string, IEntityAttribute>();
        
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        
        // Build a map of parameter ID -> mapping
        const mappingsBySourceId = new Map<string, IFieldMapping[]>();
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!mappingsBySourceId.has(sourceFieldId)) {
                mappingsBySourceId.set(sourceFieldId, []);
            }
            mappingsBySourceId.get(sourceFieldId)!.push(mapping);
        }
        
        for (const param of operationParams) {
            const paramTypeRef = param.typeReference;
            
            // Skip the DTO parameter itself
            if (paramTypeRef && paramTypeRef.isTypeFound()) {
                const paramType = paramTypeRef.getType() as MacroApi.Context.IElementApi;
                if (paramType && paramType.id === dtoElement.id) {
                    console.log(`[ANALYZE-PARAMS] Skipping DTO parameter: ${param.getName()}`);
                    continue;
                }
            }
            
            // Check this parameter for discrepancies
            const paramMappings = mappingsBySourceId.get(param.id) || [];
            
            console.log(`[ANALYZE-PARAMS] Parameter ${param.getName()}: ${paramMappings.length} mappings`);
            
            if (paramMappings.length === 0) {
                // No mapping - parameter should be deleted or is unused
                const discrepancy: IFieldDiscrepancy = {
                    id: `delete-param-${param.id}`,
                    type: "DELETE",
                    dtoFieldId: param.id,
                    dtoFieldName: param.getName(),
                    dtoFieldType: paramTypeRef?.display || "Unknown",
                    entityAttributeName: "(no mapping)",
                    icon: param.getIcon(),
                    reason: `Parameter '${param.getName()}' is not mapped to any entity attribute`
                };
                discrepancies.push(discrepancy);
                console.log(`[ANALYZE] ├─ [DELETE] Parameter ${param.getName()}: Not mapped to entity`);
            } else {
                // Check for renames and type mismatches
                for (const mapping of paramMappings) {
                    const targetAttr = entityAttrMap.get(mapping.targetAttributeId);
                    if (!targetAttr) {
                        continue;
                    }
                    
                    const paramName = param.getName();
                    const entityAttrName = targetAttr.name;
                    
                    // Check for rename - use case-sensitive for parameters
                    if (paramName !== entityAttrName) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `rename-param-${param.id}`,
                            type: "RENAME",
                            dtoFieldId: param.id,
                            dtoFieldName: paramName,
                            dtoFieldType: paramTypeRef?.display || "Unknown",
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: targetAttr.typeDisplayText,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' should be renamed to '${entityAttrName}'`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`[ANALYZE] ├─ [RENAME] Parameter ${paramName} → ${entityAttrName}`);
                    }
                    
                    // Check for type mismatch
                    const paramType = paramTypeRef?.display || "Unknown";
                    const entityType = targetAttr.typeDisplayText || "Unknown";
                    if (paramType !== entityType) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `type-param-${param.id}`,
                            type: "CHANGE_TYPE",
                            dtoFieldId: param.id,
                            dtoFieldName: paramName,
                            dtoFieldType: paramType,
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: entityType,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' type mismatch: ${paramType} vs ${entityType}`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`[ANALYZE] ├─ [CHANGE_TYPE] Parameter ${paramName}: ${paramType} → ${entityType}`);
                    }
                }
            }
        }
    }

    /**
     * Recursively detect discrepancies at all levels (root + nested associations)
     * @param parentFieldId - For nested calls, the ID of the field that contains this DTO (for NEW items)
     */
    private detectDiscrepanciesRecursive(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[],
        discrepancies: IFieldDiscrepancy[],
        depth: number = 0,
        parentFieldId?: string
    ): void {
        const indent = Array(depth).fill("  ").join("");
        
        // Get entity attributes to compare against
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map<string, IEntityAttribute>();
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        
        // Build a map of DTO field ID -> mapping
        const mappingsBySourceId = new Map<string, IFieldMapping[]>();
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!mappingsBySourceId.has(sourceFieldId)) {
                mappingsBySourceId.set(sourceFieldId, []);
            }
            mappingsBySourceId.get(sourceFieldId)!.push(mapping);
        }
        
        // Check each DTO field for discrepancies
        const childType = inferSourceElementChildType(dtoElement);
        const dtoChildren = dtoElement.getChildren(childType);
        
        for (const dtoField of dtoChildren) {
            const fieldMappings = mappingsBySourceId.get(dtoField.id) || [];
            const fieldTypeRef = dtoField.typeReference;
            
            // Check if this is a nested DTO/association field
            let isAssociationField = false;
            let nestedEntity: MacroApi.Context.IElementApi | null = null;
            let nestedMappings: IFieldMapping[] = [];
            
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType() as MacroApi.Context.IElementApi;
                // Check if field type itself maps to entity attributes through associations
                if (fieldType && (fieldType.specialization === "DTO" || fieldType.specialization === "Class")) {
                    // This might be a nested DTO that maps to an associated entity
                    nestedEntity = this.findAssociatedEntity(dtoField, entity, mappings);
                    if (nestedEntity) {
                        isAssociationField = true;
                        nestedMappings = this.getNestedMappings(mappings, dtoField.id);
                        
                        // Handle association/nested field recursively
                        console.log(`${indent}[ANALYZE-NESTED] Analyzing nested DTO field: ${dtoField.getName()} → ${nestedEntity.getName()}`);
                        this.detectDiscrepanciesRecursive(fieldType, nestedEntity, nestedMappings, discrepancies, depth + 1, dtoField.id);
                    }
                }
            }
            
            // Skip if already handled as association field
            if (isAssociationField) {
                continue;
            }
            
            // Handle root-level field
            if (fieldMappings.length === 0) {
                // No mapping found - this field should be deleted
                const discrepancy: IFieldDiscrepancy = {
                    id: `delete-${dtoField.id}`,
                    type: "DELETE",
                    dtoFieldId: dtoField.id,
                    dtoFieldName: dtoField.getName(),
                    dtoFieldType: dtoField.typeReference?.display || "Unknown",
                    entityAttributeName: "(no mapping)",
                    icon: dtoField.getIcon(),
                    reason: `Field '${dtoField.getName()}' is not mapped to any entity attribute`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [DELETE] ${dtoField.getName()}: Not mapped to entity`);
            } else {
                // Check each mapping for type matches and renames
                for (const mapping of fieldMappings) {
                    const targetAttr = entityAttrMap.get(mapping.targetAttributeId);
                    if (!targetAttr) {
                        continue;
                    }
                    
                    const dtoFieldName = dtoField.getName();
                    const entityAttrName = targetAttr.name;
                    
                    // Check for rename
                    if (dtoFieldName !== entityAttrName) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `rename-${dtoField.id}`,
                            type: "RENAME",
                            dtoFieldId: dtoField.id,
                            dtoFieldName: dtoFieldName,
                            dtoFieldType: dtoField.typeReference?.display || "Unknown",
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: targetAttr.typeDisplayText,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' should be renamed to '${entityAttrName}'`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`${indent}[ANALYZE] ├─ [RENAME] ${dtoFieldName} → ${entityAttrName}`);
                    }
                    
                    // Check for type mismatch
                    const dtoType = dtoField.typeReference?.display || "Unknown";
                    const entityType = targetAttr.typeDisplayText || "Unknown";
                    if (dtoType !== entityType) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `type-${dtoField.id}`,
                            type: "CHANGE_TYPE",
                            dtoFieldId: dtoField.id,
                            dtoFieldName: dtoFieldName,
                            dtoFieldType: dtoType,
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: entityType,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' type mismatch: ${dtoType} vs ${entityType}`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`${indent}[ANALYZE] ├─ [CHANGE_TYPE] ${dtoFieldName}: ${dtoType} → ${entityType}`);
                    }
                }
            }
        }
        
        // Check for NEW fields (entity attributes not covered by any DTO field)
        // This should run at ALL levels, not just root
        const mappedEntityIds = new Set<string>();
        for (const mapping of mappings) {
            mappedEntityIds.add(mapping.targetAttributeId);
        }
        
        for (const entityAttr of entityAttrs) {
            if (!mappedEntityIds.has(entityAttr.id) && !entityAttr.isManagedKey) {
                // Use parentFieldId if provided (nested context), otherwise use dtoElement.id (root)
                const contextId = parentFieldId || dtoElement.id;
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-${entityAttr.id}-${contextId}`,
                    type: "NEW",
                    dtoFieldId: contextId, // ID of the field/DTO where this should be added
                    dtoFieldName: "(missing)",
                    dtoFieldType: "N/A",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    icon: entityAttr.icon,
                    reason: `Entity attribute '${entityAttr.name}' is not present in DTO`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${entityAttr.name}: Entity attribute not in DTO`);
            }
        }
    }

    /**
     * Find the associated entity for a nested DTO field
     * Match by the association name (e.g., "Block1Level3" field should match "Block1Level3" association)
     */
    private findAssociatedEntity(
        dtoField: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[]
    ): MacroApi.Context.IElementApi | null {
        const dtoFieldName = dtoField.getName();
        const dtoFieldTypeRef = dtoField.typeReference;
        
        // Get the actual DTO type that this field references
        if (!dtoFieldTypeRef || !dtoFieldTypeRef.isTypeFound()) {
            return null;
        }
        
        const nestedDtoType = dtoFieldTypeRef.getType() as MacroApi.Context.IElementApi;
        if (!nestedDtoType) {
            return null;
        }
        
        // Look through entity associations to find matching target
        const associations = entity.getAssociations();
        
        for (const assoc of associations) {
            // Get association name - this should match the DTO field name or its singular form
            const assocName = (assoc as any).getName ? (assoc as any).getName() : null;
            
            // Try to match by name first (e.g., "Block1Level3" matches "Block1Level3")
            // Handle both singular and plural (e.g., "Block1Level2s" might have association "Block1Level2")
            const fieldNameSingular = dtoFieldName.replace(/s$/, ''); // Simple pluralization removal
            
            if (assocName && (assocName === dtoFieldName || assocName === fieldNameSingular)) {
                // Found matching association by name - get its target entity
                if ((assoc as any).typeReference && (assoc as any).typeReference.isTypeFound()) {
                    return (assoc as any).typeReference.getType() as MacroApi.Context.IElementApi;
                }
            }
        }
        
        return null;
    }

    /**
     * Get mappings for a nested DTO field
     * For nested fields, the source path will be: [dto_param_id, parent_field_id, nested_field_id, ...]
     * We want mappings where parent_field_id is in the path
     */
    private getNestedMappings(
        allMappings: IFieldMapping[],
        parentFieldId: string
    ): IFieldMapping[] {
        const nestedMappings: IFieldMapping[] = [];
        
        for (const mapping of allMappings) {
            if (mapping.sourcePath && mapping.sourcePath.length > 1) {
                // Check if parent field is in the path (but not the last element)
                const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
                if (parentIndex >= 0 && parentIndex < mapping.sourcePath.length - 1) {
                    // This mapping is for a nested field under this parent
                    // Create a new mapping with adjusted paths
                    const adjustedMapping: IFieldMapping = {
                        sourcePath: mapping.sourcePath.slice(parentIndex + 1), // Remove parent and ancestors
                        targetPath: mapping.targetPath, // Keep full target path for now
                        sourceFieldId: mapping.sourceFieldId,
                        targetAttributeId: mapping.targetAttributeId,
                        mappingType: mapping.mappingType,
                        mappingTypeId: mapping.mappingTypeId
                    };
                    nestedMappings.push(adjustedMapping);
                }
            }
        }
        
        return nestedMappings;
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
     * PRUNE phase: Annotate tree nodes with discrepancies, then remove branches without discrepancies
     */
    private annotateTreeWithDiscrepancies(
        node: IExtendedTreeNode,
        discrepancies: IFieldDiscrepancy[]
    ): boolean {
        // First, add NEW discrepancies as synthetic nodes to the tree
        this.addNewDiscrepancyNodes(node, discrepancies);
        
        // Create a map of element IDs that have discrepancies
        const elementIdsWithDiscrepancies = new Set<string>();
        const discrepancyByElementId = new Map<string, IFieldDiscrepancy>();
        
        for (const disc of discrepancies) {
            if (disc.dtoFieldId) {
                elementIdsWithDiscrepancies.add(disc.dtoFieldId);
                discrepancyByElementId.set(disc.dtoFieldId, disc);
            }
            if (disc.entityAttributeId) {
                elementIdsWithDiscrepancies.add(disc.entityAttributeId);
                discrepancyByElementId.set(disc.entityAttributeId, disc);
            }
        }
        
        // Recursively annotate the tree
        return this.annotateNodeRecursive(node, elementIdsWithDiscrepancies, discrepancyByElementId);
    }

    /**
     * Add NEW discrepancies as synthetic nodes to the tree
     * These represent entity attributes that don't exist in the DTO
     */
    private addNewDiscrepancyNodes(
        node: IExtendedTreeNode,
        discrepancies: IFieldDiscrepancy[]
    ): void {
        // Find NEW discrepancies that belong to this node (by parent DTO element ID)
        const newDiscrepanciesForThisNode = discrepancies.filter(d => 
            d.type === "NEW" && d.dtoFieldId === node.elementId
        );
        
        // Add them as children
        for (const newDisc of newDiscrepanciesForThisNode) {
            const newNode: IExtendedTreeNode = {
                id: newDisc.id,
                label: `${newDisc.entityAttributeName}: ${newDisc.entityAttributeType}`,
                specializationId: "structure-dto-field",
                elementId: newDisc.entityAttributeId!,
                elementType: "NEW-Field",
                originalName: newDisc.entityAttributeName,
                originalType: newDisc.entityAttributeType,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: newDisc.icon,
                discrepancy: newDisc,
                children: []
            };
            node.children!.push(newNode);
        }
        
        // Recursively process children
        if (node.children) {
            for (const child of node.children) {
                this.addNewDiscrepancyNodes(child as IExtendedTreeNode, discrepancies);
            }
        }
    }

    private annotateNodeRecursive(
        node: IExtendedTreeNode, 
        elementIdsWithDiscrepancies: Set<string>,
        discrepancyByElementId: Map<string, IFieldDiscrepancy>
    ): boolean {
        let nodeHasDiscrepancies = elementIdsWithDiscrepancies.has(node.elementId);
        
        // Attach discrepancy object if this node has one
        if (nodeHasDiscrepancies && discrepancyByElementId.has(node.elementId)) {
            node.discrepancy = discrepancyByElementId.get(node.elementId);
        }
        
        // Check children
        if (node.children && node.children.length > 0) {
            let anyChildHasDiscrepancies = false;
            for (const child of node.children) {
                const childHasDiscrepancies = this.annotateNodeRecursive(
                    child as IExtendedTreeNode, 
                    elementIdsWithDiscrepancies,
                    discrepancyByElementId
                );
                anyChildHasDiscrepancies = anyChildHasDiscrepancies || childHasDiscrepancies;
            }
            nodeHasDiscrepancies = nodeHasDiscrepancies || anyChildHasDiscrepancies;
        }
        
        // Mark this node with discrepancy info
        node.hasDiscrepancies = nodeHasDiscrepancies;
        
        return nodeHasDiscrepancies;
    }

    /**
     * Prune branches from tree that don't have discrepancies
     */
    private pruneTreeWithoutDiscrepancies(node: IExtendedTreeNode): void {
        if (!node.children) {
            return;
        }
        
        // Filter children - keep only those with discrepancies (and root/operation-param)
        node.children = node.children.filter((child) => {
            const extChild = child as IExtendedTreeNode;
            const keep = extChild.hasDiscrepancies || 
                        extChild.elementType === "Operation-Parameter" ||
                        extChild.elementType === "DTO";  // Keep root
            
            if (keep && extChild.children) {
                this.pruneTreeWithoutDiscrepancies(extChild);
            }
            return keep;
        });
    }

    /**
     * Convert the pruned tree structure to display nodes
     */
    public buildHierarchicalTreeNodes(
        sourceElement: MacroApi.Context.IElementApi,
        dtoElement: MacroApi.Context.IElementApi,
        discrepancies: IFieldDiscrepancy[]
    ): MacroApi.Context.ISelectableTreeNode[] {
        // Use the pruned tree structure that was built and annotated
        if (!this.lastStructureTree) {
            return [];
        }
        
        // Convert the tree to display nodes
        return this.convertTreeToDisplayNodes(this.lastStructureTree);
    }

    /**
     * Recursively convert tree nodes to display nodes
     */
    private convertTreeToDisplayNodes(node: IExtendedTreeNode): MacroApi.Context.ISelectableTreeNode[] {
        const result: MacroApi.Context.ISelectableTreeNode[] = [];
        
        // Process this node's children (skip root itself)
        if (node.children) {
            for (const child of node.children) {
                const extChild = child as IExtendedTreeNode;
                const displayNode = this.createDisplayNode(extChild);
                result.push(displayNode);
            }
        }
        
        return result;
    }

    /**
     * Create a display node from a tree node
     */
    private createDisplayNode(node: IExtendedTreeNode): MacroApi.Context.ISelectableTreeNode {
        const displayNode: any = {
            id: node.id,
            label: node.label,
            specializationId: node.specializationId,
            isExpanded: true,
            isSelected: false,
            icon: node.icon,
            children: []
        };
        
        // If this node has a discrepancy, attach the display function
        if (node.discrepancy) {
            const discrepancy = node.discrepancy;
            // For NEW items, always use entityAttributeName; for others use dtoFieldName
            const fieldName = discrepancy.type === "NEW" 
                ? discrepancy.entityAttributeName 
                : (discrepancy.dtoFieldName || discrepancy.entityAttributeName || "Unknown");
            displayNode.displayFunction = createDiscrepancyDisplayFunction(discrepancy, fieldName);
            displayNode.specializationId = `discrepancy-${discrepancy.type.toLowerCase()}`;
            displayNode.label = this.formatDiscrepancyLabel(discrepancy);
        }
        
        // Recursively convert children
        if (node.children && node.children.length > 0) {
            displayNode.children = [];
            for (const child of node.children) {
                const extChild = child as IExtendedTreeNode;
                const childDisplayNode = this.createDisplayNode(extChild);
                displayNode.children.push(childDisplayNode);
            }
        }
        
        return displayNode;
    }

    private formatDiscrepancyLabel(discrepancy: IFieldDiscrepancy): string {
        switch (discrepancy.type) {
            case "DELETE":
                return `[DELETE] ${discrepancy.dtoFieldName}: ${discrepancy.dtoFieldType}`;
            case "NEW":
                return `[NEW] ${discrepancy.entityAttributeName}: ${discrepancy.entityAttributeType}`;
            case "RENAME":
                return `[RENAME] ${discrepancy.dtoFieldName} → ${discrepancy.entityAttributeName}`;
            case "CHANGE_TYPE":
                return `[CHANGE_TYPE] ${discrepancy.dtoFieldName}: ${discrepancy.dtoFieldType} → ${discrepancy.entityAttributeType}`;
            default:
                return discrepancy.dtoFieldName || discrepancy.entityAttributeName || "Unknown";
        }
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
