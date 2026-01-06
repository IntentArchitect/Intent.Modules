/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

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
        for (const mapping of mappings) {
            mappedDtoToEntity.set(mapping.sourceFieldId, mapping.targetAttributeId);
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
                    reason: "Field exists in DTO but has no entity mapping"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
        }
        
        // Check for NEW, RENAMED, and TYPE_CHANGED
        for (const entityAttr of entityAttributes) {
            let found = false;
            let mappedDtoField: IDtoField | undefined;
            
            // Find if this entity attribute is mapped from any DTO field
            for (const dtoField of dtoFields) {
                if (dtoField.isMapped && mappedDtoToEntity.get(dtoField.id) === entityAttr.id) {
                    found = true;
                    mappedDtoField = dtoField;
                    break;
                }
            }
            
            if (!found) {
                // NEW: Entity attribute not in DTO
                const discrepancy: IFieldDiscrepancy = {
                    id: `new-${entityAttr.id}`,
                    type: "NEW",
                    dtoFieldId: undefined,
                    dtoFieldName: "[Missing]",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    reason: "Entity attribute does not exist in DTO"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            } else if (mappedDtoField) {
                // Found a mapping - check for RENAMED and TYPE_CHANGED
                
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
                        reason: "Field name differs from entity attribute name"
                    };
                    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                    discrepancies.push(discrepancy);
                }
                
                // TYPE_CHANGED check
                if (mappedDtoField.typeId !== entityAttr.typeId && 
                    mappedDtoField.typeDisplayText !== entityAttr.typeDisplayText) {
                    const discrepancy: IFieldDiscrepancy = {
                        id: `type-changed-${mappedDtoField.id}`,
                        type: "TYPE_CHANGED",
                        dtoFieldId: mappedDtoField.id,
                        dtoFieldName: mappedDtoField.name,
                        dtoFieldType: mappedDtoField.typeDisplayText,
                        entityAttributeId: entityAttr.id,
                        entityAttributeName: entityAttr.name,
                        entityAttributeType: entityAttr.typeDisplayText,
                        reason: "Field type differs from entity attribute type"
                    };
                    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                    discrepancies.push(discrepancy);
                }
            }
        }
        
        return discrepancies;
    }
    
    public buildTreeNodes(discrepancies: IFieldDiscrepancy[]): MacroApi.Context.ISelectableTreeNode[] {
        const nodes: MacroApi.Context.ISelectableTreeNode[] = [];
        
        for (const discrepancy of discrepancies) {
            const node: MacroApi.Context.ISelectableTreeNode = {
                id: discrepancy.id,
                label: discrepancy.dtoFieldName,
                specializationId: "sync-field-discrepancy",
                isExpanded: true,
                isSelected: false
            };
            nodes.push(node);
        }
        
        return nodes;
    }
}
