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
        
        const field = dtoElement.getChildren(ELEMENT_TYPE_NAMES.DTO_FIELD)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        if (field) {
            field.delete();
        }
    }
    
    private renameField(dtoElement: MacroApi.Context.IElementApi, discrepancy: IFieldDiscrepancy): void {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeName) return;
        
        const field = dtoElement.getChildren(ELEMENT_TYPE_NAMES.DTO_FIELD)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        if (field) {
            field.setName(discrepancy.entityAttributeName, false);
        }
    }
    
    private changeFieldType(dtoElement: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, discrepancy: IFieldDiscrepancy): void {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeId) return;
        
        const field = dtoElement.getChildren(ELEMENT_TYPE_NAMES.DTO_FIELD)
            .find(f => f.id === discrepancy.dtoFieldId);
        
        const attribute = entity.getChildren(ELEMENT_TYPE_NAMES.ATTRIBUTE)
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
        
        const attribute = entity.getChildren(ELEMENT_TYPE_NAMES.ATTRIBUTE)
            .find(a => a.id === discrepancy.entityAttributeId);
        
        if (!attribute) return;
        
        // Create new DTO field using createElement
        const newField = createElement(ELEMENT_TYPE_NAMES.DTO_FIELD, discrepancy.entityAttributeName, dtoElement.id);
        
        // Set type to match entity attribute
        if (newField.typeReference && attribute.typeReference) {
            newField.typeReference.setType(attribute.typeReference.typeId);
            newField.typeReference.setIsNullable(attribute.typeReference.isNullable);
            newField.typeReference.setIsCollection(attribute.typeReference.isCollection);
        }
        
        // Create mapping using advanced mapping API (for entity action associations)
        if (associations.length > 0) {
            const association = associations[0];
            try {
                const advancedMappings = association.getAdvancedMappings();
                
                // Find or use the first "Data Mapping" or create one
                let dataMapping = advancedMappings.find(m => m.mappingTypeId === "Data Mapping");
                
                if (!dataMapping && advancedMappings.length > 0) {
                    // Use first available mapping if no Data Mapping found
                    dataMapping = advancedMappings[0];
                }
                
                if (dataMapping) {
                    // Add mapped end with source path (DTO field) and target path (entity attribute)
                    dataMapping.addMappedEnd("Data Mapping", [newField.id], [attribute.id]);
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
}
