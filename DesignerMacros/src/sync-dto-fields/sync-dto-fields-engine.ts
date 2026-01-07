/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

// Extended tree node interface with display function support
interface IExtendedTreeNode extends MacroApi.Context.ISelectableTreeNode {
    displayFunction?: () => MacroApi.Context.IDisplayTextComponent[];
    displayMetadata?: {
        type: string;
        dtoFieldName: string;
        dtoFieldType?: string;
        entityAttributeName: string;
        entityAttributeType?: string;
        reason?: string;
    };
}

class FieldSyncEngine {
    
    public analyzeFieldDiscrepancies(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        mappings: IFieldMapping[]
    ): IFieldDiscrepancy[] {
        const discrepancies: IFieldDiscrepancy[] = [];
        
        const dtoFields = getDtoFields(dtoElement);
        const entityAttributes = getEntityAttributes(entity);
        
        // Create a map of mapped DTO fields -> Entity attributes
        const mappedDtoToEntity = new Map<string, string>();
        const mappedEntityToDto = new Map<string, string>();
        
        // SIDE EFFECT: This loop mutates dtoFields elements by setting isMapped and mappedToAttributeId
        // This is intentional for performance to avoid creating new objects
        for (const mapping of mappings) {
            // Only consider field-level mappings (ignore invocation mappings to constructors)
            const sourceField = dtoFields.find(f => f.id === mapping.sourceFieldId);
            const targetAttr = entityAttributes.find(a => a.id === mapping.targetAttributeId);
            
            if (sourceField && targetAttr) {
                mappedDtoToEntity.set(mapping.sourceFieldId, mapping.targetAttributeId);
                mappedEntityToDto.set(mapping.targetAttributeId, mapping.sourceFieldId);
                sourceField.isMapped = true;
                sourceField.mappedToAttributeId = mapping.targetAttributeId;
            }
        }
        
        // Check for DELETED fields (DTO fields not mapped to any entity attribute)
        for (const dtoField of dtoFields) {
            if (!dtoField.isMapped) {
                const discrepancy: IFieldDiscrepancy = {
                    id: `deleted-${dtoField.id}`,
                    type: "DELETED",
                    dtoFieldId: dtoField.id,
                    dtoFieldName: dtoField.name,
                    dtoFieldType: dtoField.typeDisplayText,
                    entityAttributeName: "N/A",
                    icon: dtoField.icon,
                    reason: "Field exists in DTO but has no entity mapping"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
        }
        
        // Check for NEW, RENAMED, and TYPE_CHANGED
        for (const entityAttr of entityAttributes) {
            const mappedDtoFieldId = mappedEntityToDto.get(entityAttr.id);
            
            if (!mappedDtoFieldId) {
                // Skip NEW discrepancy for unmapped managed keys (they're auto-generated or should be mapped in the association)
                if (entityAttr.isManagedKey) {
                    continue;
                }
                
                // NEW: Entity attribute not in DTO
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-${entityAttr.id}`,
                    type: "NEW",
                    dtoFieldId: undefined,
                    dtoFieldName: "[Missing]",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    reason: "Entity attribute does not exist in DTO",
                    icon: entityAttr.icon
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            } else {
                const mappedDtoField = dtoFields.find(f => f.id === mappedDtoFieldId);
                
                if (mappedDtoField) {
                    // RENAMED check
                    if (mappedDtoField.name !== entityAttr.name) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `renamed-${mappedDtoField.id}`,
                            type: "RENAMED",
                            dtoFieldId: mappedDtoField.id,
                            dtoFieldName: mappedDtoField.name,
                            entityAttributeId: entityAttr.id,
                            entityAttributeName: entityAttr.name,
                            dtoFieldType: mappedDtoField.typeDisplayText,
                            entityAttributeType: entityAttr.typeDisplayText,
                            reason: "Field name differs from entity attribute name",
                            icon: mappedDtoField.icon
                        };
                        discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                        discrepancies.push(discrepancy);
                    }
                    
                    // TYPE_CHANGED check
                    if (mappedDtoField.typeId !== entityAttr.typeId) {
                        const discrepancy: IFieldDiscrepancy = {
                            id: `type-changed-${mappedDtoField.id}`,
                            type: "TYPE_CHANGED",
                            dtoFieldId: mappedDtoField.id,
                            dtoFieldName: mappedDtoField.name,
                            dtoFieldType: mappedDtoField.typeDisplayText,
                            entityAttributeId: entityAttr.id,
                            entityAttributeName: entityAttr.name,
                            entityAttributeType: entityAttr.typeDisplayText,
                            reason: "Field type differs from entity attribute type",
                            icon: mappedDtoField.icon
                        };
                        discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                        discrepancies.push(discrepancy);
                    }
                }
            }
        }
        
        return discrepancies;
    }
    
    public buildTreeNodes(discrepancies: IFieldDiscrepancy[]): MacroApi.Context.ISelectableTreeNode[] {
        const nodes: MacroApi.Context.ISelectableTreeNode[] = [];
        
        for (const discrepancy of discrepancies) {
            // Use the actual field name, not "[Missing]"
            const displayName = discrepancy.type === "NEW" 
                ? discrepancy.entityAttributeName 
                : discrepancy.dtoFieldName;
            
            const node: IExtendedTreeNode = {
                id: discrepancy.id,
                label: displayName,
                specializationId: "sync-field-discrepancy",
                isExpanded: true,
                isSelected: false,
                icon: discrepancy.icon,
                displayFunction: discrepancy.displayFunction,
                displayMetadata: {
                    type: discrepancy.type,
                    dtoFieldName: discrepancy.dtoFieldName,
                    dtoFieldType: discrepancy.dtoFieldType,
                    entityAttributeName: discrepancy.entityAttributeName,
                    entityAttributeType: discrepancy.entityAttributeType,
                    reason: discrepancy.reason
                }
            };
            
            nodes.push(node);
        }
        
        return nodes;
    }
    
    public applySyncActions(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        selectedDiscrepancies: IFieldDiscrepancy[],
        associations: MacroApi.Context.IAssociationApi[]
    ): void {
        if (selectedDiscrepancies.length === 0) {
            return;
        }
        
        // Group discrepancies by type for ordered processing
        const deleted = selectedDiscrepancies.filter(d => d.type === "DELETED");
        const renamed = selectedDiscrepancies.filter(d => d.type === "RENAMED");
        const typeChanged = selectedDiscrepancies.filter(d => d.type === "TYPE_CHANGED");
        const newFields = selectedDiscrepancies.filter(d => d.type === "NEW");
        
        // Process DELETED first (remove fields)
        for (const discrepancy of deleted) {
            this.deleteField(dtoElement, discrepancy);
        }
        
        // Process RENAMED (update field names)
        for (const discrepancy of renamed) {
            this.renameField(dtoElement, discrepancy);
        }
        
        // Process TYPE_CHANGED (update type references)
        for (const discrepancy of typeChanged) {
            this.changeFieldType(dtoElement, entity, discrepancy);
        }
        
        // Process NEW last (create fields and mappings)
        for (const discrepancy of newFields) {
            this.createFieldWithMapping(dtoElement, entity, discrepancy, associations);
        }
    }
    
    private deleteField(dtoElement: MacroApi.Context.IElementApi, discrepancy: IFieldDiscrepancy): void {
        if (!discrepancy.dtoFieldId) return;
        
        // Dynamically determine child type
        const childType = inferSourceElementChildType(dtoElement);
        const field = dtoElement.getChildren(childType)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        if (field) {
            field.delete();
        }
    }
    
    private renameField(dtoElement: MacroApi.Context.IElementApi, discrepancy: IFieldDiscrepancy): void {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeName) return;
        
        // Dynamically determine child type
        const childType = inferSourceElementChildType(dtoElement);
        const field = dtoElement.getChildren(childType)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        if (field) {
            field.setName(discrepancy.entityAttributeName, false);
        }
    }
    
    private changeFieldType(dtoElement: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, discrepancy: IFieldDiscrepancy): void {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeId) return;
        
        // Dynamically determine child types
        const sourceChildType = inferSourceElementChildType(dtoElement);
        const targetChildType = inferTargetElementChildType(entity);
        
        const field = dtoElement.getChildren(sourceChildType)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        const attribute = entity.getChildren(targetChildType)
            .find(a => a.id === discrepancy.entityAttributeId);
        
        if (field && field.typeReference && attribute && attribute.typeReference) {
            field.typeReference.setType(attribute.typeReference.typeId);
            field.typeReference.setIsNullable(attribute.typeReference.isNullable);
            field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        }
    }
    
    private createFieldWithMapping(
        dtoElement: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        discrepancy: IFieldDiscrepancy,
        associations: MacroApi.Context.IAssociationApi[]
    ): void {
        if (!discrepancy.entityAttributeId || !discrepancy.entityAttributeName) return;
        
        // Dynamically determine target element type (not hard-coded to "Attribute")
        const targetChildType = inferTargetElementChildType(entity);
        const attribute = entity.getChildren(targetChildType)
            .find(a => a.id === discrepancy.entityAttributeId);
        
        if (!attribute) return;
        
        // Dynamically determine source element type (not hard-coded to "DTO-Field")
        const sourceChildType = inferSourceElementChildType(dtoElement);
        const newField = createElement(sourceChildType, discrepancy.entityAttributeName, dtoElement.id);
        
        // Set type to match entity attribute
        if (newField.typeReference && attribute.typeReference) {
            newField.typeReference.setType(attribute.typeReference.typeId);
            newField.typeReference.setIsNullable(attribute.typeReference.isNullable);
            newField.typeReference.setIsCollection(attribute.typeReference.isCollection);
        }
        
        // Create mapping using generic path resolution
        if (associations.length > 0) {
            const association = associations[0];
            try {
                const advancedMappings = association.getAdvancedMappings();
                
                if (advancedMappings.length > 0) {
                    const targetMapping = advancedMappings[0];
                    
                    if (targetMapping) {
                        // Extract existing mappings to learn the path structure
                        const existingMappings = extractFieldMappings([association]);
                        
                        // Analyze path structure from existing mappings
                        const pathTemplate = analyzePathStructure(existingMappings);
                        
                        let sourcePath: string[];
                        let targetPath: string[];
                        let mappingType = "Data Mapping"; // Default
                        
                        if (pathTemplate) {
                            // Use learned template to build paths generically
                            // Determine sourceRoot from first existing mapping
                            let sourceRoot: MacroApi.Context.IElementApi;
                            let targetRoot: MacroApi.Context.IElementApi;
                            
                            if (existingMappings.length > 0) {
                                // Get the actual roots from the existing mappings
                                const firstMapping = existingMappings[0];
                                sourceRoot = lookup(firstMapping.sourcePath[0]);
                                targetRoot = lookup(firstMapping.targetPath[0]);
                                
                                console.log(`=== Determined Source/Target Roots from Existing Mappings ===`);
                                console.log(`Source root from mapping: ${sourceRoot.getName()} (${sourceRoot.specialization})`);
                                console.log(`Target root from mapping: ${targetRoot.getName()} (${targetRoot.specialization})`);
                            } else {
                                // Fallback to using mapping's source/target elements
                                sourceRoot = targetMapping.getSourceElement();
                                targetRoot = targetMapping.getTargetElement();
                                console.log(`Using targetMapping.getSourceElement/getTargetElement`);
                            }
                            
                            console.log(`=== Path Building Diagnostics ===`);
                            console.log(`Template signature: ${pathTemplate.signature}`);
                            console.log(`Source root: ${sourceRoot.getName()} (${sourceRoot.specialization})`);
                            console.log(`Target root: ${targetRoot.getName()} (${targetRoot.specialization})`);
                            console.log(`DTO element: ${dtoElement?.getName()} (${dtoElement?.specialization})`);
                            console.log(`Template source depth: ${pathTemplate.sourceDepth}, element types: ${pathTemplate.sourceElementTypes.join(" > ")}`);
                            
                            const paths = buildPathUsingTemplate(
                                pathTemplate,
                                newField.id,
                                attribute.id,
                                sourceRoot,
                                targetRoot,
                                dtoElement  // Pass dtoElement for Operations with Parameters
                            );
                            
                            sourcePath = paths.sourcePath;
                            targetPath = paths.targetPath;
                            mappingType = pathTemplate.mappingType;
                            
                            console.log(`Built source path: [${sourcePath.map((id, i) => {
                                try {
                                    return `${lookup(id).getName()}(${lookup(id).specialization})`;
                                } catch {
                                    return id;
                                }
                            }).join(", ")}]`);
                            console.log(`Built target path: [${targetPath.map((id, i) => {
                                try {
                                    return `${lookup(id).getName()}(${lookup(id).specialization})`;
                                } catch {
                                    return id;
                                }
                            }).join(", ")}]`);
                        } else {
                            // Fallback: infer from source element type
                            const sourceElement = targetMapping.getSourceElement();
                            
                            if (sourceElement.specialization === "Operation") {
                                // For Operations: try to find parameter containing DTO
                                const dtoParameter = this.findDtoParameter(sourceElement, dtoElement);
                                
                                if (dtoParameter) {
                                    sourcePath = [sourceElement.id, dtoParameter.id, newField.id];
                                    targetPath = [entity.id, attribute.id];
                                } else {
                                    // Fallback: simpler path
                                    sourcePath = [sourceElement.id, newField.id];
                                    targetPath = [entity.id, attribute.id];
                                }
                            } else if (sourceElement.id === dtoElement.id) {
                                // Direct DTO/Command/Query: simple path
                                sourcePath = [newField.id];
                                targetPath = [attribute.id];
                            } else {
                                // Parent element: two-level path
                                sourcePath = [dtoElement.id, newField.id];
                                targetPath = [entity.id, attribute.id];
                            }
                        }
                        
                        targetMapping.addMappedEnd(mappingType, sourcePath, targetPath);
                    }
                }
            } catch (error) {
                console.warn("Failed to create advanced mapping:", error);
                // Fallback: try simple mapping
                try {
                    newField.setMapping(attribute.id);
                } catch (fallbackError) {
                    console.warn("Fallback mapping also failed:", fallbackError);
                }
            }
        }
    }
    
    private findDtoParameter(
        operation: MacroApi.Context.IElementApi,
        dtoElement: MacroApi.Context.IElementApi
    ): MacroApi.Context.IElementApi | null {
        const parameters = operation.getChildren(ELEMENT_TYPE_NAMES.PARAMETER);
        for (const param of parameters) {
            if (param.typeReference && param.typeReference.typeId === dtoElement.id) {
                return param;
            }
        }
        return null;
    }
}
