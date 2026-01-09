/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

async function syncDtoFields(element: MacroApi.Context.IElementApi): Promise<void> {
    
    console.log(`[SYNC] Starting sync for element: ${element.getName()} (${element.specialization})`);
    
    // Validate element
    if (!isValidSyncElement(element)) {
        throw new Error(
            `Invalid element type: The selected element must be a DTO, Command, Query, or Service Operation. The current element is a '${element.specialization}'.`
        );
    }
    
    // Extract DTO from element (handles both direct DTO/Command/Query and Operation with DTO parameter)
    const dtoElement = extractDtoFromElement(element);
    if (!dtoElement) {
        throw new Error(
            `Unable to find DTO: For Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`
        );
    }
    
    console.log(`[SYNC] ├─ DTO extracted: ${dtoElement.getName()}`);
    
    // Find action associations - use the original element if it's an Operation, otherwise use the DTO
    const elementToSearchForAssociations = element.specialization === "Operation" ? element : dtoElement;
    const associations = findAssociationsPointingToElement(elementToSearchForAssociations, dtoElement);
    
    // Try to get entity from associations
    let entity = getEntityFromAssociations(associations);
    
    // If no associations found, throw error
    if (!entity) {
        throw new Error(
            `No entity mappings found: The '${dtoElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`
        );
    }
    
    console.log(`[SYNC] ├─ Entity found: ${entity.getName()}`);
    console.log(`[SYNC] ├─ Associations: ${associations.length}`);
    
    // Extract field mappings from associations
    const fieldMappings = extractFieldMappings(associations);
    console.log(`[SYNC] ├─ Field mappings: ${fieldMappings.length}`);
    
    // Analyze discrepancies using new structure-first approach
    const engine = new FieldSyncEngine();
    const discrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings, new Set(), element);
    
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    
    // TEMPORARY: Show structure visualization even without discrepancies
    if (discrepancies.length === 0) {
        // Create a dummy structure node to show the tree in the UI for verification
        const structureNode: IFieldDiscrepancy = {
            id: `structure-view-${entity.id}`,
            type: "RENAME",  // Dummy type just for display
            dtoFieldId: dtoElement.id,
            dtoFieldName: "[Structure Visualization]",
            dtoFieldType: "Structure-Only",
            entityAttributeId: entity.id,
            entityAttributeName: `${dtoElement.getName()} → ${entity.getName()}`,
            entityAttributeType: "Structure-Only",
            icon: dtoElement.getIcon()
        };
        
        // Create display function
        const displayFunction = () => {
            return {
                displayText: "[Structure Visualization] - No discrepancies found. This shows the complete tree structure for reference."
            };
        };
        (structureNode as any).displayFunction = displayFunction;
        
        const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, [structureNode]);
        console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
        console.log(`[SYNC] └─ Showing structure visualization dialog`);
        
        // Present dialog with structure visualization
        const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, [structureNode], treeNodes);
        console.log(`[SYNC] └─ Dialog closed, no changes applied`);
        return;
    }
    
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, discrepancies);
    
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, discrepancies, treeNodes);
    
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Filter to only actual discrepancy IDs
        const discrepancyIds = new Set(discrepancies.map(d => d.id));
        const selectedDiscrepancies = discrepancies.filter(d => 
            selectedNodeIds.includes(d.id) && discrepancyIds.has(d.id)
        );
        
        console.log(`[SYNC] └─ Applying ${selectedDiscrepancies.length} sync actions`);
        engine.applySyncActions(dtoElement, entity, selectedDiscrepancies, associations);
    } else {
        console.log(`[SYNC] └─ No discrepancies selected`);
    }
}

async function presentSyncDialog(
    sourceElement: MacroApi.Context.IElementApi,
    dtoElement: MacroApi.Context.IElementApi,
    entity: MacroApi.Context.IElementApi,
    discrepancies: IFieldDiscrepancy[],
    treeNodes: MacroApi.Context.ISelectableTreeNode[]
): Promise<string[]> {
    // Determine root display based on source element type
    // If it's an Operation, show the Operation; otherwise show the DTO
    const rootElement = sourceElement.specialization === "Operation" ? sourceElement : dtoElement;
    const rootLabel = sourceElement.specialization === "Operation" 
        ? `${sourceElement.getName()} (${sourceElement.specialization})`
        : dtoElement.getName();
    
    console.log(`[DIALOG] sourceElement: ${JSON.stringify(sourceElement)}`);
    console.log(`[DIALOG] treeNodes: ${JSON.stringify(treeNodes.map(x=>{return {
        id: x.id, 
        label: x.label, 
        childrenCount: x.children ? x.children.length : 0, 
        isExpanded: x.isExpanded, 
        isSelected: x.isSelected,
        specializationId: x.specializationId
    };}))}`);

    const config: MacroApi.Context.IDynamicFormConfig = {
        title: `Synchronize ${sourceElement.getName()} with ${entity.getName()}`,
        icon: "fa-sync",
        helpText: "Select the field discrepancies you want to synchronize. The utility will create missing DTO fields, remove orphaned fields, and update mappings to match the target entity structure.",
        submitButtonText: "Done",
        minWidth: "800px",
        height: "600px",
        fields: [
            {
                id: "discrepancies",
                fieldType: "tree-view",
                label: "Field Discrepancies",
                isRequired: false,
                isHidden: false,
                treeViewOptions: {
                    rootNode: {
                        id: "root",
                        label: rootLabel,
                        specializationId: "dto-sync-root",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false,
                        icon: rootElement.getIcon()
                    },
                    height: "400px",
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "dto-sync-root",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-operation-param",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-dto-field",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-nested-field",
                            isSelectable: true,
                            autoExpand: true
                        }
                    ]
                }
            }
        ]
    };
    
    const result = await dialogService.openForm(config);
    
    if (!result) {
        return [];
    }
    
    // Tree-view returns selected IDs as a comma-separated string (single) or array (multiple)
    const selectedValue = result.discrepancies;
    
    if (!selectedValue) {
        return [];
    }
    
    // Handle both string and array formats
    let selectedIds: string[] = [];
    if (typeof selectedValue === 'string') {
        // Single selection or comma-separated string
        selectedIds = selectedValue.split(',').map(id => id.trim()).filter(id => id.length > 0);
    } else if (Array.isArray(selectedValue)) {
        // Multiple selections as array
        selectedIds = selectedValue;
    }
    
    return selectedIds;
}
