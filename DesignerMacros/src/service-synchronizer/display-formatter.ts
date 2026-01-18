/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

/**
 * Centralized tree node label building
 */
class TreeNodeLabelBuilder {
    static buildFieldLabel(name: string, type: string): string {
        return `${name}: ${type}`;
    }
    
    static buildDiscrepancyLabel(discrepancy: IFieldDiscrepancy): string {
        switch (discrepancy.type) {
            case "DELETE":
                return `[DELETE] ${discrepancy.sourceFieldName}: ${discrepancy.sourceFieldTypeName}`;
            case "ADD":
                return `[ADD] ${discrepancy.targetAttributeName}: ${discrepancy.targetAttributeTypeName}`;
            case "RENAME":
                return `[RENAME] ${discrepancy.sourceFieldName} → ${discrepancy.targetAttributeName}`;
            case "CHANGE_TYPE":
                return `[CHANGE_TYPE] ${discrepancy.sourceFieldName}: ${discrepancy.sourceFieldTypeName} → ${discrepancy.targetAttributeTypeName}`;
            default:
                return discrepancy.sourceFieldName || discrepancy.targetAttributeName || "Unknown";
        }
    }
}

// Discrepancy status colors
const DISCREPANCY_COLORS = {
    ADD: "#22c55e",      // Green
    DELETE: "#ef4444",  // Red
    RENAME: "#007777",  // Teal
    CHANGE_TYPE: "#f97316"  // Orange
} as const;

const DISCREPANCY_LABELS = {
    ADD: "[ADD]",
    DELETE: "[DELETE]",
    RENAME: "[RENAME]",
    CHANGE_TYPE: "[CHANGE TYPE]"
} as const;

type RenderContext = {
  discrepancy: IFieldDiscrepancy;
  label: string;
};

// Build full type display with modifiers (e.g., "string[]?", "int")
function buildTypeDisplay(baseType: string, isCollection: boolean, isNullable: boolean): string {
    let display = baseType || "Unknown";
    if (isCollection) display += "[*]";
    if (isNullable) display += "?";
    return display;
}

function renderNode(ctx: RenderContext): MacroApi.Context.IDisplayTextComponent[] {
  // today everything uses formatDiscrepancy, but now you have a single choke point
  // to branch later for association parents, grouping nodes, etc.
  return formatDiscrepancy(ctx.discrepancy, ctx.label);
}

function formatDiscrepancy(discrepancy: IFieldDiscrepancy, cleanFieldName: string): MacroApi.Context.IDisplayTextComponent[] {
    const components: MacroApi.Context.IDisplayTextComponent[] = [];
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    
    switch (discrepancy.type) {
        case "ADD":
            // cleanFieldName: type [ADD]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            
            const newTypeDisplay = buildTypeDisplay(
                discrepancy.targetAttributeTypeName || "",
                discrepancy.targetIsCollection || false,
                discrepancy.targetIsNullable || false
            );
            components.push({ text: newTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.ADD, color: statusInfo.color });
            break;
            
        case "RENAME":
            // cleanFieldName → targetName: type [RENAME]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            
            const targetName = discrepancy.targetAttributeName.split('.').pop() || discrepancy.targetAttributeName;
            components.push({ text: targetName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            
            const renameTypeDisplay = buildTypeDisplay(
                discrepancy.sourceFieldTypeName || "",
                discrepancy.sourceIsCollection || false,
                discrepancy.sourceIsNullable || false
            );
            components.push({ text: renameTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.RENAME, color: statusInfo.color });
            break;
            
        case "CHANGE_TYPE":
            // cleanFieldName: oldType → newType [CHANGE TYPE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            
            const oldTypeDisplay = buildTypeDisplay(
                discrepancy.sourceFieldTypeName || "",
                discrepancy.sourceIsCollection || false,
                discrepancy.sourceIsNullable || false
            );
            components.push({ text: oldTypeDisplay, cssClass: "text-highlight keyword" });
            
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            
            const newTypeChangeDisplay = buildTypeDisplay(
                discrepancy.targetAttributeTypeName || "",
                discrepancy.targetIsCollection || false,
                discrepancy.targetIsNullable || false
            );
            components.push({ text: newTypeChangeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.CHANGE_TYPE, color: statusInfo.color });
            break;
            
        case "DELETE":
            // cleanFieldName: type [DELETE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            
            const deleteTypeDisplay = buildTypeDisplay(
                discrepancy.sourceFieldTypeName || "",
                discrepancy.sourceIsCollection || false,
                discrepancy.sourceIsNullable || false
            );
            components.push({ text: deleteTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.DELETE, color: statusInfo.color });
            break;
    }
    
    return components;
}

function getDiscrepancyStatusInfo(type: "ADD" | "DELETE" | "RENAME" | "CHANGE_TYPE"): { color: string; cssClass: string } {
    switch (type) {
        case "ADD":
            return { color: DISCREPANCY_COLORS.ADD, cssClass: "keyword" };
        case "DELETE":
            return { color: DISCREPANCY_COLORS.DELETE, cssClass: "typeref" };
        case "RENAME":
            return { color: DISCREPANCY_COLORS.RENAME, cssClass: "annotation" };
        case "CHANGE_TYPE":
            return { color: DISCREPANCY_COLORS.CHANGE_TYPE, cssClass: "muted" };
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray fallback
    }
}

function createDiscrepancyDisplayFunction(discrepancy: IFieldDiscrepancy, cleanFieldName?: string): () => MacroApi.Context.IDisplayTextComponent[] {
    const fieldName = cleanFieldName || discrepancy.sourceFieldName || discrepancy.targetAttributeName;
    return () => formatDiscrepancy(discrepancy, fieldName);
}

function attachDiscrepancyDisplayFunction(discrepancy: IFieldDiscrepancy, cleanFieldName?: string): void {
    // keep name to reduce churn; the "command" is effectively:
    // "use the discrepancy formatter"
    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy, cleanFieldName);
}