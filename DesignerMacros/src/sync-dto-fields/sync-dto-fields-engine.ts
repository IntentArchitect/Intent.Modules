/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/// <reference path="../../typings/designer-common.api.d.ts" />

/**
 * Structure-first DTO field synchronization engine
 */
class FieldSyncEngine {

    private lastStructureTree: IExtendedTreeNode | null = null;
    private entityMapCache: Map<string, Map<string, IEntityAttribute>> = new Map();

    /**
     * Main entry point: Build, annotate, and prune tree structure
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
        
        this.buildStructureTree(dtoElement, entity, mappings, sourceElement);
        
        const discrepancies: IFieldDiscrepancy[] = [];
        
        const parameters = sourceElement?.getChildren("Parameter") || [];
        if (parameters.length > 0) {
            this.detectOperationParameterDiscrepancies(sourceElement!, dtoElement, entity, mappings, discrepancies);
        }
        
        // Check DTO fields recursively
        this.detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, 0);
        
        this.annotateTreeWithDiscrepancies(this.lastStructureTree!, discrepancies);
        console.log(`[ANALYZE] └─ Total discrepancies detected: ${discrepancies.length}`);
        
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
        
        const entityAttrMap = this.getOrBuildEntityAttributeMap(entity);
        const mappingsBySourceId = this.groupMappingsBySourceId(mappings);
        
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
                    
                    // Check for rename - use naming convention-aware comparison
                    // This allows "id" to match "Id" (camelCase vs PascalCase)
                    if (!namesAreEquivalent(paramName, entityAttrName)) {
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
    /**
     * Get cached entity attribute map or build it
     */
    private getOrBuildEntityAttributeMap(entity: MacroApi.Context.IElementApi): Map<string, IEntityAttribute> {
        const cached = this.entityMapCache.get(entity.id);
        if (cached) {
            return cached;
        }
        
        const map = this.buildEntityAttributeMap(entity);
        this.entityMapCache.set(entity.id, map);
        return map;
    }

    private buildEntityAttributeMap(entity: MacroApi.Context.IElementApi): Map<string, IEntityAttribute> {
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map<string, IEntityAttribute>();
        
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        
        return entityAttrMap;
    }

    private groupMappingsBySourceId(mappings: IFieldMapping[]): Map<string, IFieldMapping[]> {
        const grouped = new Map<string, IFieldMapping[]>();
        
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!grouped.has(sourceFieldId)) {
                grouped.set(sourceFieldId, []);
            }
            grouped.get(sourceFieldId)!.push(mapping);
        }
        
        return grouped;
    }

    private isComplexType(element: MacroApi.Context.IElementApi): boolean {
        return element.specialization === "DTO" || element.specialization === "Class";
    }

    private isNestedMapping(mapping: IFieldMapping, parentFieldId: string): boolean {
        if (!mapping.sourcePath || mapping.sourcePath.length <= 1) {
            return false;
        }
        
        const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
        return parentIndex >= 0 && parentIndex < mapping.sourcePath.length - 1;
    }

    private adjustMappingForNestedContext(mapping: IFieldMapping, parentFieldId: string): IFieldMapping {
        const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
        return {
            ...mapping,
            sourcePath: mapping.sourcePath.slice(parentIndex + 1)
        };
    }

    private detectDiscrepanciesRecursive(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[],
        discrepancies: IFieldDiscrepancy[],
        depth: number = 0,
        parentFieldId?: string
    ): void {
        const indent = Array(depth).fill("  ").join("");
        
        const entityAttrMap = this.getOrBuildEntityAttributeMap(entity);
        const mappingsBySourceId = this.groupMappingsBySourceId(mappings);
        
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
                if (fieldType && this.isComplexType(fieldType)) {
                    nestedEntity = this.findAssociatedEntity(dtoField, entity, mappings);
                    if (nestedEntity) {
                        isAssociationField = true;
                        nestedMappings = this.getNestedMappings(mappings, dtoField.id);
                        
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
                    
                    // Check for rename - use naming convention-aware comparison
                    // This allows "id" to match "Id" (camelCase vs PascalCase)
                    if (!namesAreEquivalent(dtoFieldName, entityAttrName)) {
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
        
        const mappedEntityIds = new Set<string>();
        for (const mapping of mappings) {
            mappedEntityIds.add(mapping.targetAttributeId);
        }
        
        const entityAttrs = getEntityAttributes(entity);
        for (const entityAttr of entityAttrs) {
            // Show NEW discrepancies for unmapped attributes, including FK attributes
            // Only exclude primary keys (which are managed keys with Primary Key stereotype)
            const isPrimaryKey = entityAttr.isManagedKey && entityAttr.hasPrimaryKeyStereotype;
            if (!mappedEntityIds.has(entityAttr.id) && !isPrimaryKey) {
                const contextId = parentFieldId || dtoElement.id;
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-${entityAttr.id}-${contextId}`,
                    type: "NEW",
                    dtoFieldId: contextId,
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

        // Check for missing associations (nested DTOs)
        // Include both source-end (this entity -> other) and target-end (other -> this entity)
        const allAssociations = entity.getAssociations("Association");
        const sourceEndAssociations = allAssociations.filter(assoc => assoc.isSourceEnd());
        const targetEndAssociations = allAssociations.filter(assoc => !assoc.isSourceEnd());
        
        console.log(`${indent}[ANALYZE-ASSOC] Found ${sourceEndAssociations.length} source-end and ${targetEndAssociations.length} target-end associations on ${entity.getName()}`);
        
        const mappedAssociationIds = new Set<string>();
        for (const mapping of mappings) {
            // Check if this mapping represents an association
            if (mapping.targetPath && mapping.targetPath.length > 0) {
                const lastTargetId = mapping.targetPath[mapping.targetPath.length - 1];
                mappedAssociationIds.add(lastTargetId);
            }
        }

        // Process source-end associations (parent -> child)
        for (const assoc of sourceEndAssociations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const targetTypeRef = assoc.typeReference;
            
            if (!assocName || !targetTypeRef || !targetTypeRef.isTypeFound()) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping source-end association - invalid name or type`);
                continue;
            }

            const targetEntity = targetTypeRef.getType() as MacroApi.Context.IElementApi;
            const assocId = assoc.id;
            
            console.log(`${indent}[ANALYZE-ASSOC] Checking source-end association ${assocName} (id: ${assocId}, mapped: ${mappedAssociationIds.has(assocId)})`);

            // Check if this association is mapped
            if (!mappedAssociationIds.has(assocId)) {
                const contextId = parentFieldId || dtoElement.id;
                const dtoName = dtoElement.getName();
                const entityName = targetEntity.getName();
                
                // Generate expected DTO name: {SourceDTO}{EntityName}Dto
                const expectedDtoName = `${dtoName}${entityName}Dto`;
                const isCollection = targetTypeRef.isCollection ? "[*]" : "";
                
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-assoc-${assocId}-${contextId}`,
                    type: "NEW",
                    dtoFieldId: contextId,
                    dtoFieldName: "(missing)",
                    dtoFieldType: `${expectedDtoName}${isCollection}`,
                    entityAttributeId: assocId,
                    entityAttributeName: assocName,
                    entityAttributeType: `${entityName}${isCollection}`,
                    icon: targetEntity.getIcon(),
                    reason: `Association '${assocName}' (type: ${entityName}) is not present in DTO. Consider adding: ${expectedDtoName}`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${assocName}: Association to ${entityName} should be added as ${expectedDtoName}`);
            }
        }
        
        // Process target-end associations (these represent inverse relationships that should also be included)
        // But only if they're navigable and represent real composite associations from the perspective of the DTO
        for (const assoc of targetEndAssociations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const typeRef = assoc.typeReference;
            
            if (!assocName || !typeRef || !typeRef.isTypeFound() || !typeRef.isNavigable) {
                continue;
            }

            // For target-end associations, check the OTHER end (source end) to determine if it's composite
            const otherEnd = assoc.getOtherEnd ? assoc.getOtherEnd() : null;
            if (!otherEnd || !otherEnd.typeReference) {
                continue;
            }

            // Composite relationship: source end should NOT be collection or nullable
            // If source end is collection or nullable, it's an aggregate and should be filtered
            const isComposite = !otherEnd.typeReference.isCollection && !otherEnd.typeReference.isNullable;
            if (!isComposite) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping target-end association ${assocName} - not composite (source is collection or nullable)`);
                continue;
            }

            const sourceEntity = typeRef.getType() as MacroApi.Context.IElementApi;
            const assocId = assoc.id;
            
            console.log(`${indent}[ANALYZE-ASSOC] Checking target-end association ${assocName} (id: ${assocId}, mapped: ${mappedAssociationIds.has(assocId)}, composite: ${isComposite})`);

            // Check if this association is mapped
            if (!mappedAssociationIds.has(assocId)) {
                const contextId = parentFieldId || dtoElement.id;
                const dtoName = dtoElement.getName();
                const entityName = sourceEntity.getName();
                
                // Generate expected DTO name: {SourceDTO}{EntityName}Dto
                const expectedDtoName = `${dtoName}${entityName}Dto`;
                const isCollection = typeRef.isCollection ? "[*]" : "";
                
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-assoc-${assocId}-${contextId}`,
                    type: "NEW",
                    dtoFieldId: contextId,
                    dtoFieldName: "(missing)",
                    dtoFieldType: `${expectedDtoName}${isCollection}`,
                    entityAttributeId: assocId,
                    entityAttributeName: assocName,
                    entityAttributeType: `${entityName}${isCollection}`,
                    icon: sourceEntity.getIcon(),
                    reason: `Association '${assocName}' (type: ${entityName}) is not present in DTO. Consider adding: ${expectedDtoName}`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${assocName}: Target-end association to ${entityName} should be added as ${expectedDtoName}`);
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
        
        const associations = entity.getAssociations();
        
        for (const assoc of associations) {
            const assocName = (assoc as any).getName ? (assoc as any).getName() : null;
            
            const fieldNameSingular = singularize(dtoFieldName);
            
            if (assocName && (assocName === dtoFieldName || assocName === fieldNameSingular)) {
                if ((assoc as any).typeReference && (assoc as any).typeReference.isTypeFound()) {
                    return (assoc as any).typeReference.getType() as MacroApi.Context.IElementApi;
                }
            }
        }
        
        return null;
    }

    private getNestedMappings(allMappings: IFieldMapping[], parentFieldId: string): IFieldMapping[] {
        return allMappings
            .filter(mapping => this.isNestedMapping(mapping, parentFieldId))
            .map(mapping => this.adjustMappingForNestedContext(mapping, parentFieldId));
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

            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType() as MacroApi.Context.IElementApi;
                if (fieldType && this.isComplexType(fieldType)) {
                    this.addDtoFieldsRecursive(fieldNode, fieldType, 2);
                }
            }
            
            rootNode.children!.push(fieldNode);
        }
        
        this.lastStructureTree = rootNode;
    }

    private addDtoFieldsRecursive(
        parentNode: IExtendedTreeNode,
        dtoElement: MacroApi.Context.IElementApi,
        depth: number
    ): void {
        const indent = Array(depth).fill("│  ").join("");
        const childType = inferSourceElementChildType(dtoElement);
        const children = dtoElement.getChildren(childType);
        
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = childTypeRef?.display || "Unknown";
            console.log(`[BUILD] ${indent}├─ ${child.getName()}: ${childTypeName}`);
            
            const childNode: IExtendedTreeNode = {
                id: child.id,
                label: TreeNodeLabelBuilder.buildFieldLabel(child.getName(), childTypeName),
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
            
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType() as MacroApi.Context.IElementApi;
                if (nestedType && this.isComplexType(nestedType)) {
                    this.addDtoFieldsRecursive(childNode, nestedType, depth + 1);
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
            // NEW discrepancies are synthetic nodes, don't mark their parent as having a discrepancy
            if (disc.type === "NEW") {
                continue;
            }
            
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
        
        if (newDiscrepanciesForThisNode.length > 0) {
            console.log(`[ANNOTATE] Adding ${newDiscrepanciesForThisNode.length} NEW nodes to ${node.originalName} (elementId: ${node.elementId})`);
        }
        
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
        // Start with pre-existing discrepancy marker (for synthetic NEW nodes)
        let nodeHasDiscrepancies = node.discrepancy !== undefined || elementIdsWithDiscrepancies.has(node.elementId);
        
        // Attach discrepancy object if this node has one
        if (nodeHasDiscrepancies && !node.discrepancy && discrepancyByElementId.has(node.elementId)) {
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
        
        // Filter children - keep only those with discrepancies
        node.children = node.children.filter((child) => {
            const extChild = child as IExtendedTreeNode;
            const keep = extChild.hasDiscrepancies || 
                        extChild.elementType === "DTO";  // Keep root DTO
            
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
        
        const displayNodes = this.convertTreeToDisplayNodes(this.lastStructureTree);
        return displayNodes;
    }

    /**
     * Recursively convert tree nodes to display nodes
     */
    private convertTreeToDisplayNodes(node: IExtendedTreeNode): MacroApi.Context.ISelectableTreeNode[] {
        const result: MacroApi.Context.ISelectableTreeNode[] = [];
        
        // Process this node's children (skip root itself)
        if (node.children) {
            console.log(`[BUILD-DISPLAY] Processing ${node.children.length} children of ${node.originalName} (elementId: ${node.elementId}, type: ${node.elementType})`);
            for (const child of node.children) {
                const extChild = child as IExtendedTreeNode;
                console.log(`[BUILD-DISPLAY]   ├─ Child: ${extChild.originalName} (elementId: ${extChild.elementId}, type: ${extChild.elementType}, hasDiscrepancy: ${!!extChild.discrepancy}, children: ${extChild.children?.length || 0})`);
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
            console.log(`[BUILD-DISPLAY]     └─ Has discrepancy: ${discrepancy.type} (dtoFieldId: ${discrepancy.dtoFieldId}, nodeId: ${node.elementId})`);
            // For NEW items, always use entityAttributeName; for others use dtoFieldName
            const fieldName = discrepancy.type === "NEW" 
                ? discrepancy.entityAttributeName 
                : (discrepancy.dtoFieldName || discrepancy.entityAttributeName || "Unknown");
            displayNode.displayFunction = createDiscrepancyDisplayFunction(discrepancy, fieldName);
            displayNode.specializationId = `discrepancy-${discrepancy.type.toLowerCase()}`;
            displayNode.label = TreeNodeLabelBuilder.buildDiscrepancyLabel(discrepancy);
        }
        
        if (node.children && node.children.length > 0) {
            console.log(`[BUILD-DISPLAY]     └─ Processing ${node.children.length} nested children`);
            displayNode.children = [];
            for (const child of node.children) {
                const extChild = child as IExtendedTreeNode;
                const childDisplayNode = this.createDisplayNode(extChild);
                displayNode.children.push(childDisplayNode);
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
