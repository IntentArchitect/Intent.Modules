/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

/**
 * Build a flat map of all tree nodes by ID for quick lookup
 */
function buildNodeMap(treeNodes: IExtendedTreeNode[]): Map<string, IExtendedTreeNode> {
    const map = new Map<string, IExtendedTreeNode>();
    
    function traverse(nodes: IExtendedTreeNode[]): void {
        for (const node of nodes) {
            map.set(node.id, node);
            
            // Recurse into children
            if (node.children && node.children.length > 0) {
                traverse(node.children as IExtendedTreeNode[]);
            }
        }
    }
    
    traverse(treeNodes);
    return map;
}

/**
 * Find all tree nodes that match the given selected IDs using the pre-built map
 */
function findSelectedNodes(nodeMap: Map<string, IExtendedTreeNode>, selectedIds: string[]): IExtendedTreeNode[] {
    const result: IExtendedTreeNode[] = [];
    
    for (const id of selectedIds) {
        const node = nodeMap.get(id);
        if (node) {
            result.push(node);
            console.log(`[FIND-NODES]   ├─ Node ${node.id}: has discrepancy = ${!!node.discrepancy}, type = ${node.discrepancy?.type}`);
        } else {
            console.log(`[FIND-NODES]   ├─ Node ${id}: NOT FOUND in map`);
        }
    }
    
    console.log(`[FIND-NODES] Found ${result.length} nodes from ${selectedIds.length} selected IDs`);
    return result;
}

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
    const discrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings, element);
    
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, discrepancies);
    
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    
    // Build node map for quick lookup after dialog (preserves discrepancy metadata)
    const nodeMap = buildNodeMap(treeNodes as IExtendedTreeNode[]);
    console.log(`[SYNC] ├─ Node map built with ${nodeMap.size} entries`);
    
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, discrepancies, treeNodes);
    
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Find the selected tree nodes using the preserved map
        const selectedNodes = findSelectedNodes(nodeMap, selectedNodeIds);
        
        console.log(`[SYNC] └─ Applying ${selectedNodes.length} sync actions`);
        engine.applySyncActions(dtoElement, entity, selectedNodes, associations);
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
                        "dto-sync-root",
                        "structure-operation-param",
                        "structure-dto-field",
                        "structure-nested-field",
                        "discrepancy-delete",
                        "discrepancy-new",
                        "discrepancy-rename",
                        "discrepancy-change_type"
                    ].map(id => ({
                        specializationId: id,
                        isSelectable: true,
                        autoExpand: true,
                        autoSelectChildren: true
                    }))
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
