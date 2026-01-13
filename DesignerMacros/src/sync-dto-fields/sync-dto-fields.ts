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
    
    // Extract working source element from user's selection
    // For Commands/Queries: the DTO is extracted from the element itself
    // For Service Operations: the DTO parameter becomes the working source
    const workingSourceElement = extractDtoFromElement(element);
    if (!workingSourceElement) {
        throw new Error(
            `Unable to find DTO: For Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`
        );
    }
    
    console.log(`[SYNC] ├─ Working source element: ${workingSourceElement.getName()}`);
    
    // The root source element is always the element the user clicked
    // This is where associations are located (either Command or Operation)
    const rootSourceElement = element;
    console.log(`[SYNC] ├─ Root source element: ${rootSourceElement.getName()} (${rootSourceElement.specialization})`);
    
    // Find action associations on the root source element
    const associations = findAssociationsPointingToElement(rootSourceElement, workingSourceElement);
    
    // Extract root target entity from associations
    const rootTargetEntity = getEntityFromAssociations(associations);
    
    // If no associations found, throw error
    if (!rootTargetEntity) {
        throw new Error(
            `No entity mappings found: The '${workingSourceElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`
        );
    }
    
    console.log(`[SYNC] ├─ Root target entity: ${rootTargetEntity.getName()}`);
    console.log(`[SYNC] ├─ Associations: ${associations.length}`);
    
    // Extract field mappings from associations
    const fieldMappings = extractFieldMappings(associations);
    console.log(`[SYNC] ├─ Field mappings: ${fieldMappings.length}`);
    
    // Analyze discrepancies using new architecture with explicit context
    // rootSourceElement: where associations are (Command or Operation)
    // rootTargetEntity: starting entity for mapping
    // workingSourceElement: where new DTO fields are created
    const engine = new FieldSyncEngine();
    const discrepancies = engine.analyzeFieldDiscrepancies(
        rootSourceElement,
        rootTargetEntity,
        workingSourceElement,
        fieldMappings
    );
    
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes();
    
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    
    // Build node map for quick lookup after dialog (preserves discrepancy metadata)
    const nodeMap = buildNodeMap(treeNodes as IExtendedTreeNode[]);
    console.log(`[SYNC] ├─ Node map built with ${nodeMap.size} entries`);
    
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(rootSourceElement, workingSourceElement, rootTargetEntity, treeNodes);
    
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Find the selected tree nodes using the preserved map
        const selectedNodes = findSelectedNodes(nodeMap, selectedNodeIds);
        
        console.log(`[SYNC] └─ Applying ${selectedNodes.length} sync actions`);
        engine.applySyncActions(rootSourceElement, rootTargetEntity, workingSourceElement, selectedNodes);
    } else {
        console.log(`[SYNC] └─ No discrepancies selected`);
    }
}

async function presentSyncDialog(
    rootSourceElement: MacroApi.Context.IElementApi,
    workingSourceElement: MacroApi.Context.IElementApi,
    rootTargetEntity: MacroApi.Context.IElementApi,
    treeNodes: MacroApi.Context.ISelectableTreeNode[]
): Promise<string[]> {
    // Determine root display based on root source element type
    // If it's an Operation, show the Operation; otherwise show the working source (Command/DTO)
    const rootElement = rootSourceElement.specialization === "Operation" ? rootSourceElement : workingSourceElement;
    const rootLabel = rootSourceElement.specialization === "Operation" 
        ? `${rootSourceElement.getName()} (${rootSourceElement.specialization})`
        : workingSourceElement.getName();
    
    console.log(`[DIALOG] rootSourceElement: ${JSON.stringify(rootSourceElement)}`);
    console.log(`[DIALOG] treeNodes: ${JSON.stringify(treeNodes.map(x=>{return {
        id: x.id, 
        label: x.label, 
        childrenCount: x.children ? x.children.length : 0, 
        isExpanded: x.isExpanded, 
        isSelected: x.isSelected,
        specializationId: x.specializationId
    };}))}`);

    const config: MacroApi.Context.IDynamicFormConfig = {
        title: `Synchronize ${rootSourceElement.getName()} with ${rootTargetEntity.getName()}`,
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
                        specializationId: "discrepancy",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false,
                        icon: rootElement.getIcon()
                    },
                    height: "400px",
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "discrepancy",
                            isSelectable: true,
                            autoExpand: true,
                            autoSelectChildren: true
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
